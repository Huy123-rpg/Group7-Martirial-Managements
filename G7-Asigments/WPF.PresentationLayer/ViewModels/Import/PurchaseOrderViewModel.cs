using BLL.BusinessLogicLayer.Services.Import;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Import;

public class PurchaseOrderViewModel : BaseViewModel
{
    private readonly IPurchaseOrderService _service = new PurchaseOrderService();

    private ObservableCollection<PurchaseOrder> _orders = [];
    private PurchaseOrder? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<PurchaseOrder> Orders { get => _orders; set => SetField(ref _orders, value); }
    public PurchaseOrder? Selected { get => _selected; set => SetField(ref _selected, value); }
    public string SearchText { get => _searchText; set { SetField(ref _searchText, value); Search(); } }

    public RelayCommand LoadCommand => new(Load);
    public RelayCommand SearchCommand => new(Search);
    public RelayCommand DeleteCommand => new(() => Delete(), () => Selected != null);

    public PurchaseOrderViewModel() => Load();

    private void Load()
    {
        var items = _service.GetAll();
        Orders = new ObservableCollection<PurchaseOrder>(items);
    }

    private void Search()
    {
        var items = string.IsNullOrWhiteSpace(SearchText)
            ? _service.GetAll()
            : _service.Search(SearchText);
        Orders = new ObservableCollection<PurchaseOrder>(items);
    }

    private void Delete()
    {
        if (Selected == null) return;
        _service.Delete(Selected.Id);
        Load();
    }
}
