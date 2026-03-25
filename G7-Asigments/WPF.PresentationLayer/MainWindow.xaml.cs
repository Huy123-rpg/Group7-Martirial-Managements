using System.Windows;
using WPF.PresentationLayer.ViewModels;

namespace WPF.PresentationLayer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // DataContext is already set in XAML, but setting it again doesn't hurt.
        // DataContext = new MainViewModel(); 
    }
}
