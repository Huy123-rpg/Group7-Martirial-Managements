using BLL.BusinessLogicLayer.Services.Scheduling;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

public class ScheduleFormViewModel : BaseViewModel
{
    private readonly IScheduleService _service = new ScheduleService();

    // ─── Mode ─────────────────────────────────────────────────────────────────
    public bool IsEditMode { get; }
    public string WindowTitle => IsEditMode ? "Sửa lịch" : "Tạo lịch mới";

    // ─── Lookup Data ──────────────────────────────────────────────────────────
    public ObservableCollection<LkpScheduleType> ScheduleTypes { get; } = [];
    public ObservableCollection<Warehouse>        Warehouses    { get; } = [];
    public ObservableCollection<User>             StaffUsers    { get; } = [];
    public ObservableCollection<Schedule>         Conflicts     { get; } = [];

    public static IReadOnlyList<string> RefTypes { get; } =
        ["(Không gắn tài liệu)", "PO", "GR", "SO", "GI"];

    // ─── Form Fields ──────────────────────────────────────────────────────────
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    private LkpScheduleType? _scheduleType;
    public LkpScheduleType? ScheduleType
    {
        get => _scheduleType;
        set => SetField(ref _scheduleType, value);
    }

    private Warehouse? _warehouse;
    public Warehouse? Warehouse
    {
        get => _warehouse;
        set
        {
            SetField(ref _warehouse, value);
            RefreshStaffList();
            RefreshConflicts();
        }
    }

    private DateTime _startDate = DateTime.Today;
    public DateTime StartDate
    {
        get => _startDate;
        set { SetField(ref _startDate, value); RebuildStartTime(); RefreshConflicts(); }
    }

    private string _startHour = DateTime.Now.AddHours(1).ToString("HH:mm");
    public string StartHour
    {
        get => _startHour;
        set { SetField(ref _startHour, value); RebuildStartTime(); RefreshConflicts(); }
    }

    private DateTime _startTime = DateTime.Now.AddHours(1);

    private void RebuildStartTime()
    {
        _startTime = TimeSpan.TryParse(_startHour, out var ts)
            ? _startDate.Date + ts
            : _startDate.Date;
    }

    private User? _assignedTo;
    public User? AssignedTo
    {
        get => _assignedTo;
        set { SetField(ref _assignedTo, value); RefreshConflicts(); }
    }

    private string _selectedRefType = "(Không gắn tài liệu)";
    public string SelectedRefType
    {
        get => _selectedRefType;
        set { SetField(ref _selectedRefType, value); OnPropertyChanged(nameof(HasRefType)); }
    }

    private string? _refId;
    public string? RefId
    {
        get => _refId;
        set => SetField(ref _refId, value);
    }

    // ─── Recurring ────────────────────────────────────────────────────────────
    public record RecurrenceOption(string Code, string Label);

    public static IReadOnlyList<RecurrenceOption> RecurrenceOptions { get; } =
    [
        new("WEEKLY",  "Hàng tuần"),
        new("DAILY",   "Hàng ngày"),
        new("MONTHLY", "Hàng tháng"),
    ];

    private bool _isRecurring;
    public bool IsRecurring
    {
        get => _isRecurring;
        set { SetField(ref _isRecurring, value); OnPropertyChanged(nameof(ShowRecurrence)); }
    }

    public bool ShowRecurrence => IsRecurring && !IsEditMode;

    private RecurrenceOption _recurrenceOption = new("WEEKLY", "Hàng tuần");
    public RecurrenceOption SelectedRecurrence
    {
        get => _recurrenceOption;
        set => SetField(ref _recurrenceOption, value);
    }

    private int _recurrenceCount = 4;
    public int RecurrenceCount
    {
        get => _recurrenceCount;
        set => SetField(ref _recurrenceCount, value);
    }

    // ─── Computed ─────────────────────────────────────────────────────────────
    public bool HasRefType      => SelectedRefType != "(Không gắn tài liệu)";
    public bool HasConflicts    => Conflicts.Count > 0;
    public string? ErrorMessage { get => _errorMessage; set => SetField(ref _errorMessage, value); }
    private string? _errorMessage;

    // ─── Result ───────────────────────────────────────────────────────────────
    public bool Saved { get; private set; }
    private readonly Guid? _editId;

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand SaveCommand   => new(Save,   CanSave);
    public RelayCommand CancelCommand => new(Cancel);

    // ─── Constructor ─────────────────────────────────────────────────────────
    public ScheduleFormViewModel() : this(null) { }

    public ScheduleFormViewModel(Schedule? existing)
    {
        IsEditMode = existing != null;
        _editId    = existing?.Id;

        LoadLookups();

        if (existing != null)
            PopulateFromExisting(existing);
        else if (SessionManager.IsManager)
            PreSelectManagerWarehouse();
    }

    // ─── Private Methods ──────────────────────────────────────────────────────
    private void LoadLookups()
    {
        foreach (var t in _service.GetScheduleTypes()) ScheduleTypes.Add(t);

        var allWarehouses = _service.GetWarehouses();
        if (SessionManager.IsManager)
            allWarehouses = allWarehouses.Where(w => w.ManagerId == SessionManager.CurrentUser!.Id);

        foreach (var w in allWarehouses) Warehouses.Add(w);

        // Staff list nạp sau khi có kho (RefreshStaffList sẽ dùng kho đang chọn)
        RefreshStaffList();
    }

    private void RefreshStaffList()
    {
        var currentAssigned = AssignedTo;
        StaffUsers.Clear();

        var users = _warehouse != null
            ? _service.GetStaffByWarehouse(_warehouse.Id)
            : _service.GetStaffUsers();

        foreach (var u in users) StaffUsers.Add(u);

        // Giữ lại người đã chọn nếu vẫn còn trong danh sách
        AssignedTo = StaffUsers.FirstOrDefault(u => u.Id == currentAssigned?.Id);
    }

    private void PreSelectManagerWarehouse()
    {
        Warehouse = Warehouses.FirstOrDefault(
            w => w.ManagerId == SessionManager.CurrentUser!.Id);
    }

    private void PopulateFromExisting(Schedule s)
    {
        Title         = s.Title;
        Description   = s.Description;
        _startDate = s.StartTime.LocalDateTime.Date;
        _startHour = s.StartTime.LocalDateTime.ToString("HH:mm");
        _startTime = s.StartTime.LocalDateTime;

        ScheduleType = ScheduleTypes.FirstOrDefault(t => t.TypeId == s.ScheduleType);
        Warehouse    = Warehouses.FirstOrDefault(w => w.Id == s.WarehouseId);
        AssignedTo   = StaffUsers.FirstOrDefault(u => u.Id == s.AssignedTo);

        if (!string.IsNullOrWhiteSpace(s.RefType))
        {
            SelectedRefType = s.RefType;
            RefId           = s.RefId?.ToString();
        }
    }

    private void RefreshConflicts()
    {
        Conflicts.Clear();
        if (Warehouse == null) return;

        var end = (DateTimeOffset)_startTime.AddHours(1);

        var conflicts = _service.CheckConflicts(
            Warehouse.Id,
            AssignedTo?.Id,
            (DateTimeOffset)_startTime,
            end,
            _editId);

        foreach (var c in conflicts) Conflicts.Add(c);
        OnPropertyChanged(nameof(HasConflicts));
    }

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Title) &&
        ScheduleType != null &&
        Warehouse    != null;

    private void Save()
    {
        ErrorMessage = null;
        try
        {
            var schedule = BuildSchedule();

            if (IsEditMode)
                _service.Update(schedule);
            else
                _service.Create(schedule);

            Saved = true;
            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.InnerException?.InnerException?.Message
                        ?? ex.InnerException?.Message
                        ?? ex.Message;
        }
    }

    private Schedule BuildSchedule()
    {
        return new Schedule
        {
            Id              = _editId ?? Guid.Empty,
            Title           = Title.Trim(),
            Description     = Description,
            ScheduleType    = ScheduleType!.TypeId,
            StatusCode      = ScheduleService.StatusDraft,
            WarehouseId     = Warehouse!.Id,
            StartTime       = (DateTimeOffset)_startTime,
            EndTime         = null,
            AssignedTo      = AssignedTo?.Id,
            RefType         = HasRefType ? SelectedRefType : null,
            RefId           = HasRefType && Guid.TryParse(RefId, out var rid) ? rid : null,
            CreatedBy       = SessionManager.CurrentUser!.Id,
            IsRecurring     = IsRecurring && !IsEditMode,
            RecurrenceRule  = (IsRecurring && !IsEditMode)
                ? $"{SelectedRecurrence.Code}:{RecurrenceCount}"
                : null,
        };
    }

    private void Cancel() => CloseWindow();

    private void CloseWindow()
    {
        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
        {
            if (w.DataContext == this) { w.Close(); return; }
        }
    }
}
