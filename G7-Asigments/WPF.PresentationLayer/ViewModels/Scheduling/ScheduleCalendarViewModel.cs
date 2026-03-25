using BLL.BusinessLogicLayer.Services.Scheduling;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using WPF.PresentationLayer.Helpers;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

public class ScheduleCalendarViewModel : BaseViewModel
{
    private readonly IScheduleService _service;
    private static readonly CultureInfo _vi = new("vi-VN");

    private DateTime _currentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private List<Schedule> _monthSchedules = [];

    public string MonthLabel => _currentMonth.ToString("MMMM yyyy", _vi);

    public ObservableCollection<CalendarDayModel> Days { get; } = [];

    private CalendarDayModel? _selectedDay;
    public CalendarDayModel? SelectedDay
    {
        get => _selectedDay;
        set => SetField(ref _selectedDay, value);
    }

    private Schedule? _selectedSchedule;
    public Schedule? SelectedSchedule
    {
        get => _selectedSchedule;
        set => SetField(ref _selectedSchedule, value);
    }

    public RelayCommand PrevMonthCommand  => new(() => { _currentMonth = _currentMonth.AddMonths(-1);  Reload(); });
    public RelayCommand NextMonthCommand  => new(() => { _currentMonth = _currentMonth.AddMonths(1);   Reload(); });
    public RelayCommand TodayCommand      => new(() => { _currentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1); Reload(); });
    public RelayCommand RefreshCommand    => new(Reload);

    public RelayCommand SelectScheduleCommand => new(p =>
    {
        if (p is ScheduleSlot slot)
            SelectedSchedule = _monthSchedules.FirstOrDefault(s => s.Id == slot.Id);
    });

    public ScheduleCalendarViewModel() : this(App.ServiceProvider.GetRequiredService<IScheduleService>()) { }

    public ScheduleCalendarViewModel(IScheduleService service)
    {
        _service = service;
        Reload();
    }

    public void Reload()
    {
        OnPropertyChanged(nameof(MonthLabel));
        Days.Clear();
        SelectedDay      = null;
        SelectedSchedule = null;

        var schedules = _service.GetByMonth(_currentMonth.Year, _currentMonth.Month).ToList();
        _monthSchedules  = schedules;
        var staffDict    = _service.GetStaffUsers().ToDictionary(u => u.Id, u => u.FullName);

        // Monday-based grid start
        int startOffset = ((int)_currentMonth.DayOfWeek + 6) % 7;
        var start = _currentMonth.AddDays(-startOffset);

        for (int i = 0; i < 42; i++)
        {
            var date = start.AddDays(i);

            var slots = schedules
                .Where(s => s.StartTime.Date == date.Date)
                .Select(s => new ScheduleSlot
                {
                    Id             = s.Id,
                    Title          = s.Title,
                    StatusCode     = s.StatusCode,
                    AssignedToId   = s.AssignedTo,
                    AssignedToName = s.AssignedTo.HasValue &&
                                     staffDict.TryGetValue(s.AssignedTo.Value, out var n) ? n : "",
                })
                .ToList();

            Days.Add(new CalendarDayModel
            {
                Date           = date,
                IsCurrentMonth = date.Month == _currentMonth.Month,
                Slots          = slots,
            });
        }
    }
}
