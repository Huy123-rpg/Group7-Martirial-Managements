using BLL.BusinessLogicLayer.Services.WarehouseConfig;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Admin;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class WarehouseConfigViewModel : BaseViewModel
{
    private readonly IWarehouseConfigService _service = new WarehouseConfigService();

    // ─── Permission ───────────────────────────────────────────────────────────
    public bool IsAdmin       => SessionManager.IsAdmin;
    public bool IsManagerOnly => SessionManager.IsManager;

    // ─── Warehouse list ───────────────────────────────────────────────────────
    private ObservableCollection<Warehouse> _warehouses = [];
    public ObservableCollection<Warehouse> Warehouses
    {
        get => _warehouses;
        set => SetField(ref _warehouses, value);
    }

    private Warehouse? _selectedWarehouse;
    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse;
        set
        {
            SetField(ref _selectedWarehouse, value);
            OnPropertyChanged(nameof(CanEditWarehouse));
            LoadZones();
        }
    }

    private string _warehouseSearch = string.Empty;
    public string WarehouseSearch
    {
        get => _warehouseSearch;
        set { SetField(ref _warehouseSearch, value); ApplyWarehouseFilter(); }
    }

    private List<Warehouse> _allWarehouses = [];

    // ─── Zone list ────────────────────────────────────────────────────────────
    private ObservableCollection<WarehouseZone> _zones = [];
    public ObservableCollection<WarehouseZone> Zones
    {
        get => _zones;
        set => SetField(ref _zones, value);
    }

    private WarehouseZone? _selectedZone;
    public WarehouseZone? SelectedZone
    {
        get => _selectedZone;
        set { SetField(ref _selectedZone, value); OnPropertyChanged(nameof(CanEditZone)); }
    }

    // ─── Permission-derived visibility ────────────────────────────────────────
    public bool CanEditWarehouse => IsAdmin && SelectedWarehouse != null;
    public bool CanEditZone      => IsAdmin && SelectedZone != null;

    // ─── Commands — Admin ─────────────────────────────────────────────────────
    public RelayCommand AddWarehouseCommand      => new(OpenAddWarehouse,    () => IsAdmin);
    public RelayCommand EditWarehouseCommand     => new(OpenEditWarehouse,   () => CanEditWarehouse);
    public RelayCommand ToggleWarehouseCommand   => new(ToggleWarehouse,     () => CanEditWarehouse);
    public RelayCommand DeleteWarehouseCommand   => new(DeleteWarehouse,     () => CanEditWarehouse);
    public RelayCommand AddZoneCommand           => new(OpenAddZone,         () => IsAdmin && SelectedWarehouse != null);
    public RelayCommand EditZoneCommand          => new(OpenEditZone,        () => CanEditZone);
    public RelayCommand ToggleZoneCommand        => new(ToggleZone,          () => CanEditZone);
    public RelayCommand DeleteZoneCommand        => new(DeleteZone,          () => CanEditZone);

    // ─── Commands — Manager ───────────────────────────────────────────────────
    public RelayCommand ProposeWarehouseCommand  => new(ProposeWarehouseChange, () => IsManagerOnly && SelectedWarehouse != null);

    public WarehouseConfigViewModel() => LoadWarehouses();

    // ─── Load ─────────────────────────────────────────────────────────────────
    private void LoadWarehouses()
    {
        var all = _service.GetAllWarehouses().ToList();

        // Manager chỉ thấy kho mình quản lý
        if (IsManagerOnly)
        {
            var myId = SessionManager.CurrentUser?.Id;
            all = all.Where(w => w.ManagerId == myId).ToList();
        }

        _allWarehouses = all;
        ApplyWarehouseFilter();
        SelectedWarehouse = null;
        Zones = [];
    }

    private void ApplyWarehouseFilter()
    {
        var result = _allWarehouses.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(WarehouseSearch))
            result = result.Where(w =>
                w.Code.Contains(WarehouseSearch, StringComparison.OrdinalIgnoreCase) ||
                w.Name.Contains(WarehouseSearch, StringComparison.OrdinalIgnoreCase) ||
                (w.City ?? "").Contains(WarehouseSearch, StringComparison.OrdinalIgnoreCase));
        Warehouses = new ObservableCollection<Warehouse>(result);
    }

    private void LoadZones()
    {
        if (SelectedWarehouse == null) { Zones = []; return; }
        var zones = _service.GetZonesByWarehouse(SelectedWarehouse.Id).ToList();
        Zones = new ObservableCollection<WarehouseZone>(zones);
        SelectedZone = null;
    }

    // ─── Admin — Warehouse actions ────────────────────────────────────────────
    private void OpenAddWarehouse()
    {
        var vm  = new WarehouseFormViewModel(_service);
        var win = new WarehouseFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) LoadWarehouses();
    }

    private void OpenEditWarehouse()
    {
        if (SelectedWarehouse == null) return;
        var vm  = new WarehouseFormViewModel(_service, SelectedWarehouse);
        var win = new WarehouseFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) LoadWarehouses();
    }

    private void ToggleWarehouse()
    {
        if (SelectedWarehouse == null) return;
        string action = SelectedWarehouse.IsActive ? "vô hiệu hóa" : "kích hoạt";
        if (MessageBox.Show($"Xác nhận {action} kho \"{SelectedWarehouse.Name}\"?",
            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try
        {
            _service.ToggleWarehouseActive(SelectedWarehouse.Id);
            LoadWarehouses();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void DeleteWarehouse()
    {
        if (SelectedWarehouse == null) return;
        if (MessageBox.Show($"Bạn có chắc chắn muốn xóa kho \"{SelectedWarehouse.Name}\" không?\n\nLưu ý: Việc này sẽ ĐỒNG THỜI XÓA MỌI DỮ LIỆU liên quan đến kho này (Zone, Tồn kho, Hoá đơn Nhập/Xuất, Đơn hàng, Lịch trình, v.v.).\nHành động này không thể hoàn tác!",
            "Cảnh báo xóa dữ liệu nghiêm trọng", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        try
        {
            _service.DeleteWarehouse(SelectedWarehouse.Id);
            LoadWarehouses();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    // ─── Admin — Zone actions ─────────────────────────────────────────────────
    private void OpenAddZone()
    {
        if (SelectedWarehouse == null) return;
        var vm  = new ZoneFormViewModel(_service, SelectedWarehouse.Id);
        var win = new ZoneFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) LoadZones();
    }

    private void OpenEditZone()
    {
        if (SelectedZone == null) return;
        var vm  = new ZoneFormViewModel(_service, SelectedZone);
        var win = new ZoneFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) LoadZones();
    }

    private void ToggleZone()
    {
        if (SelectedZone == null) return;
        string action = SelectedZone.IsActive ? "vô hiệu hóa" : "kích hoạt";
        if (MessageBox.Show($"Xác nhận {action} zone \"{SelectedZone.ZoneCode}\"?",
            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try
        {
            _service.ToggleZoneActive(SelectedZone.Id);
            LoadZones();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void DeleteZone()
    {
        if (SelectedZone == null) return;
        if (MessageBox.Show($"Xác nhận XOÁ VĨNH VIỄN zone \"{SelectedZone.ZoneCode}\"?\nLưu ý: Không thể hoàn tác!",
            "Cảnh báo xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        try
        {
            _service.DeleteZone(SelectedZone.Id);
            LoadZones();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    // ─── Manager — Propose change ─────────────────────────────────────────────
    private void ProposeWarehouseChange()
    {
        if (SelectedWarehouse == null) return;
        var vm  = new WarehouseFormViewModel(_service, SelectedWarehouse, proposeMode: true);
        var win = new WarehouseFormWindow { DataContext = vm };
        win.ShowDialog();
    }
}
