using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Auth;
using BLL.BusinessLogicLayer.Services.Email;
using DAL.DataAccessLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Auth;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService  _authService  = new AuthService();
    private readonly IEmailService _emailService = new EmailService();
    private readonly UnitOfWork    _uow          = UnitOfWork.Instance;

    // ─── Fields ───────────────────────────────────────────────────────────────
    private string _fullName        = string.Empty;
    private string _email           = string.Empty;
    private string _password        = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage    = string.Empty;
    private LkpUserRole? _selectedRole;

    public string FullName        { get => _fullName;        set => SetField(ref _fullName, value); }
    public string Email           { get => _email;           set => SetField(ref _email, value); }
    public string Password        { get => _password;        set => SetField(ref _password, value); }
    public string ConfirmPassword { get => _confirmPassword; set => SetField(ref _confirmPassword, value); }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { SetField(ref _errorMessage, value); OnPropertyChanged(nameof(ErrorVisibility)); }
    }

    public Visibility ErrorVisibility
        => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    public LkpUserRole? SelectedRole
    {
        get => _selectedRole;
        set => SetField(ref _selectedRole, value);
    }

    // ─── Roles (exclude ADMIN) ────────────────────────────────────────────────
    public ObservableCollection<LkpUserRole> Roles { get; } = new();

    /// <summary>
    /// true  = standalone (sau khi tạo → chuyển sang LoginWindow)
    /// false = admin mode (sau khi tạo → đóng dialog, quay lại caller)
    /// </summary>
    public bool IsStandalone { get; set; } = true;

    public Visibility BackToLoginVisibility
        => IsStandalone ? Visibility.Visible : Visibility.Collapsed;

    public RegisterViewModel()
    {
        LoadRoles();
    }

    private void LoadRoles()
    {
        var roles = _uow.UserRoles
            .GetAll()
            .Where(r => r.RoleCode != "ADMIN")
            .OrderBy(r => r.RoleName);

        foreach (var r in roles)
            Roles.Add(r);

        SelectedRole = Roles.FirstOrDefault();
    }

    // ─── Register Command ─────────────────────────────────────────────────────
    public RelayCommand RegisterCommand => new(Register, CanRegister);

    private bool CanRegister()
        => !string.IsNullOrWhiteSpace(FullName)
        && !string.IsNullOrWhiteSpace(Email)
        && !string.IsNullOrWhiteSpace(Password)
        && !string.IsNullOrWhiteSpace(ConfirmPassword)
        && SelectedRole != null;

    private async void Register()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return;
        }

        User? user;
        try
        {
            user = _authService.Register(FullName, Email, Password, SelectedRole!.RoleId, out string error);
            if (user == null) { ErrorMessage = error; return; }
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.InnerException?.Message
                      ?? ex.InnerException?.Message
                      ?? ex.Message;
            ErrorMessage = $"Lỗi khi tạo tài khoản: {detail}";
            return;
        }

        // Gửi email — chờ kết quả trước khi đóng cửa sổ
        string emailNote;
        try
        {
            await _emailService.SendWelcomeEmailAsync(Email, user.FullName, Password);
            emailNote = "Hệ thống đã gửi email thông báo đến người dùng.";
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            emailNote = $"(Không gửi được email: {detail})";
        }

        MessageBox.Show(
            $"Tài khoản \"{Email}\" đã được tạo thành công.\n" +
            emailNote + "\n" +
            "Người dùng sẽ được yêu cầu đổi mật khẩu khi đăng nhập lần đầu.",
            "Tạo tài khoản thành công",
            MessageBoxButton.OK,
            MessageBoxImage.None);

        if (IsStandalone)
            NavigateToLogin();
        else
            CloseCurrentWindow();
    }

    // ─── Navigate to Login ────────────────────────────────────────────────────
    public RelayCommand NavigateToLoginCommand => new(NavigateToLogin);

    private void NavigateToLogin()
    {
        var loginWindow = new Views.Auth.LoginWindow();
        loginWindow.Show();
        CloseCurrentWindow();
    }

    // ─── Helper ───────────────────────────────────────────────────────────────
    private static void CloseCurrentWindow()
    {
        foreach (Window w in Application.Current.Windows)
        {
            if (w is Views.Auth.RegisterWindow) { w.Close(); return; }
        }
    }
}
