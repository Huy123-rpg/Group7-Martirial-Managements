using System.Windows;
using WPF.PresentationLayer.ViewModels.Auth;

namespace WPF.PresentationLayer.Views.Auth;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        PasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        };
    }
}
