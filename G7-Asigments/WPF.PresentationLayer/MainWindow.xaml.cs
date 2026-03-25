using System.Windows;
using WPF.PresentationLayer.Helpers;
using WPF.PresentationLayer.ViewModels;

namespace WPF.PresentationLayer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        ApplySidebarVisibility();
    }

    private void ApplySidebarVisibility()
    {
        bool isAdmin   = SessionManager.IsAdmin;
        bool isManager = SessionManager.IsManager;
        bool isStaff   = SessionManager.IsStaff;

        // Admin menu
        var adminVis = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderAdmin.Visibility = adminVis;
        BtnUsers.Visibility        = adminVis;

        // Schedule menu (Admin + Manager)
        var scheduleVis = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderSchedule.Visibility = scheduleVis;
        BtnSchedule.Visibility        = scheduleVis;

        // My schedule (Staff)
        var myScheduleVis = isStaff ? Visibility.Visible : Visibility.Collapsed;
        MenuHeaderMySchedule.Visibility = myScheduleVis;
        BtnMySchedule.Visibility        = myScheduleVis;
    }
}
