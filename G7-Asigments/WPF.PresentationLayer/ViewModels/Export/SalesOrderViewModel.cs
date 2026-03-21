using BLL.BusinessLogicLayer.Services.Export;
using DAL.DataAccessLayer.Models._Export;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Export;

public class SalesOrderViewModel : BaseViewModel
{
    private readonly ISalesOrderService _service = new SalesOrderService();

    private ObservableCollection<SalesOrder> _orders = [];
    private SalesOrder? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<SalesOrder> Orders { get => _orders; set => SetField(ref _orders, value); }
    public SalesOrder? Selected { get => _selected; set => SetField(ref _selected, value); }
    public string SearchText { get => _searchText; set { SetField(ref _searchText, value); Search(); } }

    public RelayCommand LoadCommand => new(Load);
    public RelayCommand SearchCommand => new(Search);
    public RelayCommand DeleteCommand => new(() => Delete(), () => Selected != null);

    public SalesOrderViewModel() => Load();

    private void Load()
    {
        var items = _service.GetAll();
        Orders = new ObservableCollection<SalesOrder>(items);
    }

    private void Search()
    {
        var items = string.IsNullOrWhiteSpace(SearchText)
            ? _service.GetAll()
            : _service.Search(SearchText);
        Orders = new ObservableCollection<SalesOrder>(items);
    }

    private void Delete()
    {
        if (Selected == null) return;
        _service.Delete(Selected.Id);
        Load();
    }
}
