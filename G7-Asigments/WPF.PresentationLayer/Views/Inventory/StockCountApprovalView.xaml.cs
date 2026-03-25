using WPF.PresentationLayer.ViewModels.Inventory;
using System.Windows.Controls;

namespace WPF.PresentationLayer.Views.Inventory;

public partial class StockCountApprovalView : UserControl
{
    public StockCountApprovalView()
    {
        InitializeComponent();
        DataContext = new StockCountApprovalViewModel();
    }
}
