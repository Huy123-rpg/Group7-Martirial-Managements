using DAL.DataAccessLayer.Models;
using System.Windows;
using WPF.PresentationLayer.ViewModels.Inventory;

namespace WPF.PresentationLayer.Views.Inventory;

public partial class StockCountExecutionDetailWindow : Window
{
    public StockCountExecutionDetailWindow(StockCountSession session)
    {
        InitializeComponent();
        DataContext = new StockCountExecutionDetailViewModel(session);
    }

    private void BtnComplete_Click(object sender, RoutedEventArgs e)
    {
        // Binded CompleteCommand will run first. Then we optionally close if it successfully locked CanEdit.
        var vm = DataContext as StockCountExecutionDetailViewModel;
        if (vm != null && !vm.CanEdit)
        {
            this.Close();
        }
    }
}
