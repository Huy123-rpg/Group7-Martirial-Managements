using BLL.BusinessLogicLayer.Services.Auth;
using DAL.DataAccessLayer.Models;
using System;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService = new AuthService();

    private string _email    = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Email        { get => _email;        set => SetField(ref _email, value); }
    public string Password     { get => _password;     set => SetField(ref _password, value); }
    public string ErrorMessage { get => _errorMessage; set { SetField(ref _errorMessage, value); OnPropertyChanged(nameof(ErrorVisibility)); } }

    public Visibility ErrorVisibility
        => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    // ─── Login Command ────────────────────────────────────────────────────────
    public RelayCommand LoginCommand => new(Login, CanLogin);

    private bool CanLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    private void Login()
    {
        User? user;
        try { user = _authService.Login(Email, Password); }
        catch (Exception ex) { ErrorMessage = $"Lỗi kết nối DB: {ex.Message}"; return; }

        if (user == null)
        {
            ErrorMessage = "Email hoặc mật khẩu không đúng.";
            return;
        }

        SessionManager.Login(user);
        ErrorMessage = string.Empty;

        // First login → bắt buộc đổi mật khẩu
        if (user.LastLoginAt == null)
        {
            var changeWin = new Views.Auth.ChangePasswordWindow();
            if (changeWin.DataContext is ChangePasswordViewModel cpVm)
                cpVm.UserId = user.Id;

            bool changed = changeWin.ShowDialog() == true;
            if (!changed)
            {
                SessionManager.Logout();
                ErrorMessage = "Vui lòng đổi mật khẩu để tiếp tục.";
                return;
            }
        }

        try
        {
            _authService.UpdateLastLogin(user.Id);
            var main = new MainWindow();
            main.Show();
            CloseCurrentWindow();
        }
        catch (Exception ex)
        {
            SessionManager.Logout();
            ErrorMessage = $"Không thể mở ứng dụng: {ex.Message}";
        }
    }

    // ─── Helper ───────────────────────────────────────────────────────────────
    private static void CloseCurrentWindow()
    {
        foreach (Window w in Application.Current.Windows)
        {
            if (w is Views.Auth.LoginWindow) { w.Close(); return; }
        }
    }
}
