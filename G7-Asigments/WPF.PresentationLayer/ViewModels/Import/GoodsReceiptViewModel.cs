using BLL.BusinessLogicLayer.Services.Import;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Import;

public class GoodsReceiptViewModel : BaseViewModel
{
    private readonly IGoodsReceiptService _service = new GoodsReceiptService();

    private ObservableCollection<GoodsReceipt> _receipts = [];
    private GoodsReceipt? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<GoodsReceipt> Receipts { get => _receipts; set => SetField(ref _receipts, value); }
    public GoodsReceipt? Selected { get => _selected; set => SetField(ref _selected, value); }
    public string SearchText { get => _searchText; set { SetField(ref _searchText, value); Search(); } }

    public RelayCommand LoadCommand => new(Load);
    public RelayCommand SearchCommand => new(Search);

    public GoodsReceiptViewModel() => Load();

    private void Load()
    {
        var items = _service.GetAll();
        Receipts = new ObservableCollection<GoodsReceipt>(items);
    }

    private void Search()
    {
        var items = string.IsNullOrWhiteSpace(SearchText)
            ? _service.GetAll()
            : _service.Search(SearchText);
        Receipts = new ObservableCollection<GoodsReceipt>(items);
    }
}
