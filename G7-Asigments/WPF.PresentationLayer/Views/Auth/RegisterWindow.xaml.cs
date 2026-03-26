using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.ViewModels.Auth;

namespace WPF.PresentationLayer.Views.Auth;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();

        PasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = PasswordBox.Password;
        };

        ConfirmPasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is RegisterViewModel vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        };
    }
}
