using System.Windows.Controls;

namespace WPF.PresentationLayer.Views.Inventory;

public partial class StockCountExecutionListView : UserControl
{
    public StockCountExecutionListView()
    {
        InitializeComponent();
        DataContext = new ViewModels.Inventory.StockCountExecutionListViewModel();
    }
}
