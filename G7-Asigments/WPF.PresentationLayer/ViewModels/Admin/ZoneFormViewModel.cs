using BLL.BusinessLogicLayer.Services.WarehouseConfig;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class ZoneFormViewModel : BaseViewModel
{
    private readonly IWarehouseConfigService _service;
    private readonly Guid _warehouseId;
    private readonly Guid? _editId;

    public bool IsEditMode  { get; }
    public string WindowTitle => IsEditMode ? "Sửa zone" : "Thêm zone mới";

    // ─── Form Fields ──────────────────────────────────────────────────────────
    private string _zoneCode = string.Empty;
    public string ZoneCode { get => _zoneCode; set => SetField(ref _zoneCode, value); }

    private string? _zoneName;
    public string? ZoneName { get => _zoneName; set => SetField(ref _zoneName, value); }

    private LkpZoneType? _selectedZoneType;
    public LkpZoneType? SelectedZoneType { get => _selectedZoneType; set => SetField(ref _selectedZoneType, value); }

    private decimal? _capacityM3;
    public decimal? CapacityM3 { get => _capacityM3; set => SetField(ref _capacityM3, value); }

    private int? _capacityPallet;
    public int? CapacityPallet { get => _capacityPallet; set => SetField(ref _capacityPallet, value); }

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    public ObservableCollection<LkpZoneType> ZoneTypes { get; } = [];

    // ─── Result ───────────────────────────────────────────────────────────────
    public bool Saved { get; private set; }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetField(ref _errorMessage, value); }
    public Visibility ErrorVisibility => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand SaveCommand   => new(Save,   CanSave);
    public RelayCommand CancelCommand => new(Cancel);

    // ─── Constructors ─────────────────────────────────────────────────────────
    public ZoneFormViewModel(IWarehouseConfigService service, Guid warehouseId)
    {
        _service     = service;
        _warehouseId = warehouseId;
        IsEditMode   = false;
        LoadLookups();
    }

    public ZoneFormViewModel(IWarehouseConfigService service, WarehouseZone existing)
    {
        _service     = service;
        _warehouseId = existing.WarehouseId;
        _editId      = existing.Id;
        IsEditMode   = true;
        LoadLookups();
        Populate(existing);
    }

    private void LoadLookups()
    {
        foreach (var t in _service.GetZoneTypes()) ZoneTypes.Add(t);
    }

    private void Populate(WarehouseZone z)
    {
        ZoneCode       = z.ZoneCode;
        ZoneName       = z.ZoneName;
        CapacityM3     = z.CapacityM3;
        CapacityPallet = z.CapacityPallet;
        SelectedZoneType = ZoneTypes.FirstOrDefault(t => t.TypeId == z.ZoneType);
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(ZoneCode) && SelectedZoneType != null;

    private void Save()
    {
        ErrorMessage = null;
        try
        {
            var zone = new WarehouseZone
            {
                Id             = _editId ?? Guid.Empty,
                WarehouseId    = _warehouseId,
                ZoneCode       = ZoneCode,
                ZoneName       = ZoneName,
                ZoneType       = SelectedZoneType!.TypeId,
                CapacityM3     = CapacityM3,
                CapacityPallet = CapacityPallet,
            };

            if (IsEditMode)
                _service.UpdateZone(zone);
            else
                _service.CreateZone(zone);

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
