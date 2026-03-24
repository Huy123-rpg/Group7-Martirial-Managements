using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.ViewModels.Scheduling;

namespace WPF.PresentationLayer.Views.Scheduling;

public partial class ScheduleCalendarView : UserControl
{
    public ScheduleCalendarView()
    {
        InitializeComponent();
        IsVisibleChanged += OnVisibleChanged;
    }

    private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue && DataContext is ScheduleCalendarViewModel vm)
            vm.Reload();
    }
}
