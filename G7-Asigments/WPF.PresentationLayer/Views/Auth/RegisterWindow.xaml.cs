using System.Windows;
using System.Windows.Controls;
using WPF.PresentationLayer.ViewModels.Auth;

namespace WPF.PresentationLayer.Views.Auth;

public partial class RegisterWindow : Window
{
    public RegisterWindow(RegisterViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        txtPassword.PasswordChanged += (_, _) =>
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = txtPassword.Password;
        };

        txtConfirmPassword.PasswordChanged += (_, _) =>
        {
            if (DataContext is RegisterViewModel vm)
                vm.ConfirmPassword = txtConfirmPassword.Password;
        };
    }
}
