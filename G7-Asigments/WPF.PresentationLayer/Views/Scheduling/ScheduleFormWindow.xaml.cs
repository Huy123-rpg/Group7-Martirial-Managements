using System.Windows;
using WPF.PresentationLayer.ViewModels.Scheduling;

namespace WPF.PresentationLayer.Views.Scheduling;

public partial class ScheduleFormWindow : Window
{
    public ScheduleFormWindow(ScheduleFormViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
