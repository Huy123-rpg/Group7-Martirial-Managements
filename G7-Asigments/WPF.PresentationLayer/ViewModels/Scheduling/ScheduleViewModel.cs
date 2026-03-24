using BLL.BusinessLogicLayer.Services.Scheduling;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.Views.Scheduling;
using WPF.PresentationLayer.Views.Shared;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

public class ScheduleViewModel : BaseViewModel
{
    private readonly IScheduleService _service = new ScheduleService();

    // ─── List State ───────────────────────────────────────────────────────────
    private ObservableCollection<Schedule> _schedules = [];
    private Schedule? _selected;
    private string _searchText   = string.Empty;
    private string _filterStatus = "ALL";
    private DateTime? _filterFrom;
    private DateTime? _filterTo;

    public ObservableCollection<Schedule> Schedules
    {
        get => _schedules;
        set => SetField(ref _schedules, value);
    }

    public Schedule? Selected
    {
        get => _selected;
        set
        {
            SetField(ref _selected, value);
            OnPropertyChanged(nameof(CanCancel));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged(nameof(CanMarkMissed));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set { SetField(ref _searchText, value); ApplyFilter(); }
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set { SetField(ref _filterStatus, value); ApplyFilter(); }
    }

    public DateTime? FilterFrom
    {
        get => _filterFrom;
        set { SetField(ref _filterFrom, value); ApplyFilter(); }
    }

    public DateTime? FilterTo
    {
        get => _filterTo;
        set { SetField(ref _filterTo, value); ApplyFilter(); }
    }

    // ─── Permission Computed Properties ──────────────────────────────────────
    public bool IsManagerOrAdmin => SessionManager.IsAdmin || SessionManager.IsManager;

    public bool CanCancel =>
        Selected != null &&
        Selected.StatusCode != ScheduleService.StatusCompleted &&
        Selected.StatusCode != ScheduleService.StatusCancelled &&
        Selected.StatusCode != ScheduleService.StatusMissed &&
        IsManagerOrAdmin;

    public bool CanEdit =>
        Selected != null &&
        Selected.StatusCode == ScheduleService.StatusDraft &&
        IsManagerOrAdmin;

    public bool CanDelete =>
        Selected != null &&
        Selected.StatusCode == ScheduleService.StatusDraft &&
        IsManagerOrAdmin;

    public bool CanMarkMissed =>
        Selected != null &&
        Selected.StatusCode == ScheduleService.StatusDraft &&
        Selected.StartTime < DateTimeOffset.Now &&
        IsManagerOrAdmin;

    // ─── Status Filter Options ────────────────────────────────────────────────
    public record StatusOption(string Code, string Label);

    public static IReadOnlyList<StatusOption> StatusDisplayOptions { get; } =
    [
        new("ALL",         "Tất cả"),
        new("scheduled",   "Đã lên lịch"),
        new("in_progress", "Đang thực hiện"),
        new("done",        "Hoàn thành"),
        new("cancelled",   "Đã hủy"),
        new("missed",      "Bỏ lỡ"),
    ];

    // ─── All loaded schedules (before filter) ─────────────────────────────────
    private List<Schedule> _allSchedules = [];

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand LoadCommand         => new(Load);
    public RelayCommand CreateCommand       => new(OpenCreate,     () => IsManagerOrAdmin);
    public RelayCommand EditCommand         => new(OpenEdit,       () => CanEdit);
    public RelayCommand CancelScheduleCommand => new(CancelSelected, () => CanCancel);
    public RelayCommand MarkMissedCommand   => new(MarkMissed,     () => CanMarkMissed);
    public RelayCommand DeleteCommand       => new(Delete,         () => CanDelete);

    public ScheduleViewModel() => Load();

    // ─── Load ────────────────────────────────────────────────────────────────
    private void Load()
    {
        _allSchedules = (SessionManager.IsAdmin || SessionManager.IsManager
            ? _service.GetAll()
            : Enumerable.Empty<Schedule>()).ToList();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var result = _allSchedules.AsEnumerable();

        if (FilterStatus != "ALL")
            result = result.Where(s => s.StatusCode == FilterStatus);

        if (FilterFrom.HasValue)
            result = result.Where(s => s.StartTime >= (DateTimeOffset)FilterFrom.Value);

        if (FilterTo.HasValue)
            result = result.Where(s => s.StartTime <= (DateTimeOffset)FilterTo.Value.AddDays(1));

        if (!string.IsNullOrWhiteSpace(SearchText))
            result = result.Where(s =>
                s.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (s.Description ?? "").Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Schedules = new ObservableCollection<Schedule>(result.OrderBy(s => s.StartTime));
    }

    // ─── Create / Edit ────────────────────────────────────────────────────────
    private void OpenCreate()
    {
        var vm  = new ScheduleFormViewModel();
        var win = new ScheduleFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) Load();
    }

    private void OpenEdit()
    {
        if (Selected == null) return;
        var vm  = new ScheduleFormViewModel(Selected);
        var win = new ScheduleFormWindow { DataContext = vm };
        win.ShowDialog();
        if (vm.Saved) Load();
    }

    // ─── Status Actions ───────────────────────────────────────────────────────
    private void CancelSelected()
    {
        if (Selected == null) return;

        var dialog = new InputDialog("Nhập lý do hủy lịch:", "Hủy lịch");
        if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.Result)) return;

        try
        {
            _service.CancelSchedule(Selected.Id, SessionManager.CurrentUser!.Id, dialog.Result);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void MarkMissed()
    {
        if (Selected == null) return;
        var confirm = MessageBox.Show(
            $"Đánh dấu lịch \"{Selected.Title}\" là bỏ lỡ?",
            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            _service.MarkMissed(Selected.Id, SessionManager.CurrentUser!.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Delete()
    {
        if (Selected == null) return;
        var result = MessageBox.Show(
            $"Xóa lịch \"{Selected.Title}\"?",
            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            _service.Delete(Selected.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
