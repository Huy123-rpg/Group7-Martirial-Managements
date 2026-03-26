using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.WarehouseConfig;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class WarehouseFormViewModel : BaseViewModel
{
    private readonly IWarehouseConfigService _service;
    private readonly bool _isProposeMode;
    private readonly Guid? _editId;

    // ─── Mode ─────────────────────────────────────────────────────────────────
    public bool IsEditMode    { get; }
    public bool IsProposeMode => _isProposeMode;
    public bool IsReadOnly    => _isProposeMode;
    public bool IsEditable    => !_isProposeMode;

    public string WindowTitle => _isProposeMode ? "Đề xuất thay đổi kho"
                               : IsEditMode     ? "Sửa kho"
                                                : "Thêm kho mới";

    // ─── Form Fields ──────────────────────────────────────────────────────────
    private string _code = string.Empty;
    public string Code { get => _code; set => SetField(ref _code, value); }

    private string _name = string.Empty;
    public string Name { get => _name; set => SetField(ref _name, value); }

    private string? _address;
    public string? Address { get => _address; set => SetField(ref _address, value); }

    private string? _city;
    public string? City { get => _city; set => SetField(ref _city, value); }

    private User? _selectedManager;
    public User? SelectedManager { get => _selectedManager; set => SetField(ref _selectedManager, value); }

    private LkpCostingMethod? _selectedCostingMethod;
    // CostingMethod field removed (not shown in UI)

    // ─── Propose mode extras ──────────────────────────────────────────────────
    private string? _proposeNote;
    public string? ProposeNote { get => _proposeNote; set => SetField(ref _proposeNote, value); }

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    public ObservableCollection<User> Managers { get; } = [];

    // ─── Result ───────────────────────────────────────────────────────────────
    public bool Saved { get; private set; }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetField(ref _errorMessage, value); }
    public Visibility ErrorVisibility => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand SaveCommand   => new(Save,   CanSave);
    public RelayCommand CancelCommand => new(Cancel);

    // ─── Constructors ─────────────────────────────────────────────────────────
    public WarehouseFormViewModel(IWarehouseConfigService service)
    {
        _service = service;
        IsEditMode = false;
        _isProposeMode = false;
        LoadLookups();
    }

    public WarehouseFormViewModel(IWarehouseConfigService service, Warehouse existing, bool proposeMode = false)
    {
        _service = service;
        IsEditMode = true;
        _isProposeMode = proposeMode;
        _editId = existing.Id;
        LoadLookups();
        Populate(existing);
    }

    private void LoadLookups()
    {
        foreach (var m in _service.GetManagers()) Managers.Add(m);
    }

    private void Populate(Warehouse w)
    {
        Code    = w.Code;
        Name    = w.Name;
        Address = w.Address;
        City    = w.City;
        SelectedManager = Managers.FirstOrDefault(m => m.Id == w.ManagerId);
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);

    private void Save()
    {
        ErrorMessage = null;
        try
        {
            if (_isProposeMode)
            {
                // Manager proposes — create in-app Notifications for ALL Admins
                var uow     = UnitOfWork.Instance;
                var manager = SessionManager.CurrentUser;
                string managerName = manager?.FullName ?? "Manager";
                string title = $"📋 Đề xuất thay đổi kho: {Code}";
                string body  = $"{managerName} đề xuất thay đổi kho \"{Name}\"." +
                               (string.IsNullOrWhiteSpace(ProposeNote)
                                    ? ""
                                    : $"\nGhi chú: {ProposeNote}");

                var adminIds = uow.Users
                    .Find(u => u.IsActive && u.RoleId == 1)
                    .Select(u => u.Id)
                    .ToList();

                if (adminIds.Count == 0)
                    throw new InvalidOperationException("Không tìm thấy Admin nào trong hệ thống.");

                foreach (var adminId in adminIds)
                {
                    uow.Notifications.Add(new Notification
                    {
                        Id        = Guid.NewGuid(),
                        UserId    = adminId,
                        Title     = title,
                        Body      = body,
                        Channel   = "IN_APP",
                        RefType   = "WAREHOUSE_PROPOSAL",
                        RefId     = manager?.Id,
                        IsRead    = false,
                        SentAt    = DateTimeOffset.UtcNow,
                        CreatedAt = DateTimeOffset.UtcNow,
                    });
                }
                uow.Save();
                MessageBox.Show($"Đề xuất đã được gửi đến {adminIds.Count} Admin.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Saved = true;
                CloseWindow();
                return;
            }

            var warehouse = new Warehouse
            {
                Id        = _editId ?? Guid.Empty,
                Code      = Code,
                Name      = Name,
                Address   = Address,
                City      = City,
                ManagerId = SelectedManager?.Id,
            };

            if (IsEditMode)
                _service.UpdateWarehouse(warehouse);
            else
                _service.CreateWarehouse(warehouse);

            Saved = true;
            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            OnPropertyChanged(nameof(ErrorVisibility));
        }
    }

    private void Cancel() => CloseWindow();

    private void CloseWindow()
    {
        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            if (w.DataContext == this) { w.Close(); return; }
    }
}
