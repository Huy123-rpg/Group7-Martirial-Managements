using BLL.BusinessLogicLayer.Services.WarehouseConfig;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

/// <summary>
/// ViewModel cho Tab "Phân công nhân viên kho" trong ScheduleListView.
/// Chỉ Admin mới thấy và sử dụng tab này.
/// </summary>
public class WarehouseStaffViewModel : BaseViewModel
{
    private readonly IWarehouseStaffService _service = new WarehouseStaffService();
    private readonly IWarehouseConfigService _warehouseService = new WarehouseConfigService();

    // ─── Danh sách kho ───────────────────────────────────────────────────────
    private ObservableCollection<Warehouse> _warehouses = [];
    private Warehouse? _selectedWarehouse;

    public ObservableCollection<Warehouse> Warehouses
    {
        get => _warehouses;
        set => SetField(ref _warehouses, value);
    }

    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse;
        set
        {
            SetField(ref _selectedWarehouse, value);
            LoadStaff();
            LoadAvailableStaff();
        }
    }

    // ─── Staff đã gán ────────────────────────────────────────────────────────
    private ObservableCollection<WarehouseStaff> _assignedStaff = [];
    private WarehouseStaff? _selectedAssigned;

    public ObservableCollection<WarehouseStaff> AssignedStaff
    {
        get => _assignedStaff;
        set => SetField(ref _assignedStaff, value);
    }

    public WarehouseStaff? SelectedAssigned
    {
        get => _selectedAssigned;
        set => SetField(ref _selectedAssigned, value);
    }

    // ─── Staff chưa gán (để thêm) ────────────────────────────────────────────
    private ObservableCollection<User> _availableStaff = [];
    private User? _selectedAvailable;

    public ObservableCollection<User> AvailableStaff
    {
        get => _availableStaff;
        set => SetField(ref _availableStaff, value);
    }

    public User? SelectedAvailable
    {
        get => _selectedAvailable;
        set => SetField(ref _selectedAvailable, value);
    }

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand AssignCommand  => new(Assign,  () => SelectedWarehouse != null && SelectedAvailable != null);
    public RelayCommand RemoveCommand  => new(Remove,  () => SelectedWarehouse != null && SelectedAssigned  != null);

    public WarehouseStaffViewModel() => LoadWarehouses();

    // ─── Load ─────────────────────────────────────────────────────────────────
    private void LoadWarehouses()
    {
        var list = _warehouseService.GetAllWarehouses();
        Warehouses = new ObservableCollection<Warehouse>(list);
        SelectedWarehouse = Warehouses.FirstOrDefault();
    }

    private void LoadStaff()
    {
        if (SelectedWarehouse == null) { AssignedStaff = []; return; }
        var rows = _service.GetByWarehouse(SelectedWarehouse.Id);
        AssignedStaff = new ObservableCollection<WarehouseStaff>(rows);
    }

    private void LoadAvailableStaff()
    {
        if (SelectedWarehouse == null) { AvailableStaff = []; return; }
        var rows = _service.GetAvailableStaff(SelectedWarehouse.Id);
        AvailableStaff = new ObservableCollection<User>(rows);
        SelectedAvailable = null;
    }

    // ─── Actions ──────────────────────────────────────────────────────────────
    private void Assign()
    {
        if (SelectedWarehouse == null || SelectedAvailable == null) return;
        try
        {
            _service.Assign(SelectedWarehouse.Id, SelectedAvailable.Id,
                            SessionManager.CurrentUser!.Id);
            LoadStaff();
            LoadAvailableStaff();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Remove()
    {
        if (SelectedWarehouse == null || SelectedAssigned == null) return;

        var confirm = MessageBox.Show(
            $"Gỡ \"{SelectedAssigned.User?.FullName}\" khỏi kho \"{SelectedWarehouse.Name}\"?",
            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            _service.Remove(SelectedWarehouse.Id, SelectedAssigned.UserId);
            LoadStaff();
            LoadAvailableStaff();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
