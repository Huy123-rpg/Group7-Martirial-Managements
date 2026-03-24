using System.Windows.Controls;

namespace WPF.PresentationLayer.Views.Import;

public partial class PurchaseOrderListView : UserControl
{
    public PurchaseOrderListView() => InitializeComponent();

    private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var window = new PurchaseOrderAddWindow
        {
            Owner = System.Windows.Window.GetWindow(this)
        };
        
        if (window.ShowDialog() == true)
        {
            if (DataContext is ViewModels.Import.PurchaseOrderViewModel vm)
            {
                vm.LoadCommand.Execute(null);
            }
        }
    }

    private void BtnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewModels.Import.PurchaseOrderViewModel vm && vm.Selected != null)
        {
            var window = new PurchaseOrderAddWindow(vm.Selected)
            {
                Owner = System.Windows.Window.GetWindow(this)
            };
            
            if (window.ShowDialog() == true)
            {
                vm.LoadCommand.Execute(null);
            }
        }
    }
}
