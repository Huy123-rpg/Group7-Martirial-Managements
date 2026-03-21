using BLL.BusinessLogicLayer.Services.Export;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Export;

public class GoodsIssueViewModel : BaseViewModel
{
    private readonly IGoodsIssueService _service = new GoodsIssueService();

    private ObservableCollection<GoodsIssue> _issues = [];
    private GoodsIssue? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<GoodsIssue> Issues { get => _issues; set => SetField(ref _issues, value); }
    public GoodsIssue? Selected { get => _selected; set => SetField(ref _selected, value); }
    public string SearchText { get => _searchText; set { SetField(ref _searchText, value); Search(); } }

    public RelayCommand LoadCommand => new(Load);
    public RelayCommand SearchCommand => new(Search);

    public GoodsIssueViewModel() => Load();

    private void Load()
    {
        var items = _service.GetAll();
        Issues = new ObservableCollection<GoodsIssue>(items);
    }

    private void Search()
    {
        var items = string.IsNullOrWhiteSpace(SearchText)
            ? _service.GetAll()
            : _service.Search(SearchText);
        Issues = new ObservableCollection<GoodsIssue>(items);
    }
}
