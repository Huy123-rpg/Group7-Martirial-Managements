using BLL.BusinessLogicLayer.Services.Scheduling;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

public class MyScheduleViewModel : BaseViewModel
{
    private readonly IScheduleService _service;

    // ─── List State ───────────────────────────────────────────────────────────
    private ObservableCollection<Schedule> _schedules = [];
    private Schedule? _selected;
    private bool _showHistory;

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
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanComplete));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(StatusColor));
        }
    }

    public bool ShowHistory
    {
        get => _showHistory;
        set { SetField(ref _showHistory, value); Load(); }
    }

    // ─── Permission Computed ──────────────────────────────────────────────────
    public bool CanStart =>
        Selected != null &&
        Selected.StatusCode == ScheduleService.StatusDraft &&
        Selected.AssignedTo == SessionManager.CurrentUser?.Id;

    public bool CanComplete =>
        Selected != null &&
        Selected.StatusCode == ScheduleService.StatusInProgress &&
        Selected.AssignedTo == SessionManager.CurrentUser?.Id;

    // ─── Status Display Helpers ───────────────────────────────────────────────
    public string StatusLabel => Selected?.StatusCode switch
    {
        ScheduleService.StatusDraft      => "Đã lên lịch — sẵn sàng bắt đầu",
        ScheduleService.StatusInProgress => "Đang thực hiện",
        ScheduleService.StatusCompleted  => "Hoàn thành",
        ScheduleService.StatusCancelled  => "Đã hủy",
        ScheduleService.StatusMissed     => "Bỏ lỡ",
        _ => string.Empty
    };

    public string StatusColor => Selected?.StatusCode switch
    {
        ScheduleService.StatusDraft      => "#1976D2",
        ScheduleService.StatusInProgress => "#FF9800",
        ScheduleService.StatusCompleted  => "#388E3C",
        ScheduleService.StatusCancelled  => "#D32F2F",
        ScheduleService.StatusMissed     => "#FF6F00",
        _ => "#9E9E9E"
    };

    // ─── Labels ───────────────────────────────────────────────────────────────
    public string TodayLabel     => $"Lịch của tôi — {DateTime.Today:dd/MM/yyyy}";
    public string ToggleLabel    => ShowHistory ? "Xem lịch đang hoạt động" : "Xem lịch sử";

    // ─── Commands ─────────────────────────────────────────────────────────────
    public RelayCommand LoadCommand     => new(Load);
    public RelayCommand StartCommand    => new(Start,    () => CanStart);
    public RelayCommand CompleteCommand => new(Complete, () => CanComplete);
    public RelayCommand ToggleHistoryCommand => new(() =>
    {
        ShowHistory = !ShowHistory;
        OnPropertyChanged(nameof(ToggleLabel));
    });

    public MyScheduleViewModel() : this(App.ServiceProvider.GetRequiredService<IScheduleService>()) { }

    public MyScheduleViewModel(IScheduleService service)
    {
        _service = service;
        Load();
    }

    // ─── Load ─────────────────────────────────────────────────────────────────
    private void Load()
    {
        if (SessionManager.CurrentUser == null) return;
        var userId = SessionManager.CurrentUser.Id;
        var items  = ShowHistory
            ? _service.GetAllAssignedTo(userId)
                      .Where(s => s.StatusCode == ScheduleService.StatusCompleted ||
                                  s.StatusCode == ScheduleService.StatusCancelled  ||
                                  s.StatusCode == ScheduleService.StatusMissed)
            : _service.GetAssignedTo(userId);

        Schedules = new ObservableCollection<Schedule>(items);
    }

    // ─── Status Actions ───────────────────────────────────────────────────────
    private void Start()
    {
        if (Selected == null) return;
        try
        {
            _service.StartSchedule(Selected.Id, SessionManager.CurrentUser!.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Complete()
    {
        if (Selected == null) return;
        var confirm = MessageBox.Show(
            $"Xác nhận hoàn thành lịch \"{Selected.Title}\"?",
            "Hoàn thành", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            _service.CompleteSchedule(Selected.Id, SessionManager.CurrentUser!.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
