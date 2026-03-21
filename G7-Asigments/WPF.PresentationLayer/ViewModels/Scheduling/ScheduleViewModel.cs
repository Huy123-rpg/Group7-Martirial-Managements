using BLL.BusinessLogicLayer.Services.Scheduling;
using DAL.DataAccessLayer.Model;
using System.Collections.ObjectModel;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Scheduling;

public class ScheduleViewModel : BaseViewModel
{
    private readonly IScheduleService _service = new ScheduleService();

    private ObservableCollection<Schedule> _schedules = [];
    private Schedule? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<Schedule> Schedules { get => _schedules; set => SetField(ref _schedules, value); }
    public Schedule? Selected { get => _selected; set => SetField(ref _selected, value); }
    public string SearchText { get => _searchText; set { SetField(ref _searchText, value); Search(); } }

    public RelayCommand LoadCommand => new(Load);
    public RelayCommand SearchCommand => new(Search);
    public RelayCommand DeleteCommand => new(() => Delete(), () => Selected != null);

    public ScheduleViewModel() => Load();

    private void Load()
    {
        var items = _service.GetAll();
        Schedules = new ObservableCollection<Schedule>(items);
    }

    private void Search()
    {
        var items = string.IsNullOrWhiteSpace(SearchText)
            ? _service.GetAll()
            : _service.Search(SearchText);
        Schedules = new ObservableCollection<Schedule>(items);
    }

    private void Delete()
    {
        if (Selected == null) return;
        _service.Delete(Selected.Id);
        Load();
    }
}
