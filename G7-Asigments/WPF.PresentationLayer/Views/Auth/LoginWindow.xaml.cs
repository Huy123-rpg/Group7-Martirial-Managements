using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.ViewModels.Auth;

namespace WPF.PresentationLayer.Views.Auth;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        // Sync PasswordBox → ViewModel khi người dùng gõ
        PasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        };
    }
}
