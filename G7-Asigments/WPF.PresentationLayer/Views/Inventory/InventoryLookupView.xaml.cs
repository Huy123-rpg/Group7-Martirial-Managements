using System.Windows.Controls;

namespace WPF.PresentationLayer.Views.Inventory;

public partial class InventoryLookupView : UserControl
{
    public InventoryLookupView()
    {
        InitializeComponent();
        DataContext = new ViewModels.Inventory.InventoryLookupViewModel();
    }
}
