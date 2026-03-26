using BLL.BusinessLogicLayer.Services.InventoryManagement;
using DAL.DataAccessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Inventory;

public class InventoryLookupViewModel : BaseViewModel
{
    private readonly IInventoryLookupService _service;
    
    // Default filter choice
    private readonly Warehouse _allWarehousesOption = new Warehouse { Id = Guid.Empty, Name = "--- Tất cả kho ---" };

    public ObservableCollection<DAL.DataAccessLayer.Models.Inventory> Inventories { get; set; } = new();
    public ObservableCollection<Warehouse> Warehouses { get; set; } = new();

    private Warehouse? _selectedWarehouse;
    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse;
        set
        {
            _selectedWarehouse = value;
            OnPropertyChanged(nameof(SelectedWarehouse));
            LoadInventories();
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    public ICommand SearchCommand { get; }

    public InventoryLookupViewModel()
    {
        _service = new InventoryLookupService();
        SearchCommand = new RelayCommand(LoadInventories);
        
        LoadWarehouses();
        LoadInventories();
    }

    private void LoadWarehouses()
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return;

        Warehouses.Clear();
        
        // Let user see exactly the warehouses permitted for them
        var allowed = _service.GetPermittedWarehouses(user.Id, user.RoleId);

        // Add 'All' option if they have access to more than 1
        // Usually staff might be assigned to multiple, Manager just 1, Admin all
        Warehouses.Add(_allWarehousesOption);

        foreach (var w in allowed)
            Warehouses.Add(w);

        SelectedWarehouse = _allWarehousesOption;
    }

    private void LoadInventories()
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return;

        Inventories.Clear();

        Guid? filterId = SelectedWarehouse?.Id == Guid.Empty ? null : SelectedWarehouse?.Id;

        var data = _service.GetInventories(user.Id, user.RoleId, filterId, SearchText);
        foreach (var i in data)
            Inventories.Add(i);
    }
}
