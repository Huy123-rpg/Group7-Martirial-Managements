using System.Windows;
using WPF.PresentationLayer.ViewModels.Auth;

namespace WPF.PresentationLayer.Views.Auth;

public partial class ChangePasswordWindow : Window
{
    public ChangePasswordWindow()
    {
        InitializeComponent();

        CurrentPasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.CurrentPassword = CurrentPasswordBox.Password;
        };

        NewPasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.NewPassword = NewPasswordBox.Password;
        };

        ConfirmPasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is ChangePasswordViewModel vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        };
    }
}
