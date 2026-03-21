using BLL.BusinessLogicLayer.Core;
using BLL.BusinessLogicLayer.Services.Auth;
using DAL.DataAccessLayer.Models._Lookup;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Auth;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService = new AuthService();
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── Fields ───────────────────────────────────────────────────────────────
    private string _fullName        = string.Empty;
    private string _email           = string.Empty;
    private string _username        = string.Empty;
    private string _password        = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage    = string.Empty;
    private LkpUserRole? _selectedRole;

    public string FullName        { get => _fullName;        set => SetField(ref _fullName, value); }
    public string Email           { get => _email;           set => SetField(ref _email, value); }
    public string Username        { get => _username;        set => SetField(ref _username, value); }
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
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password)
        && !string.IsNullOrWhiteSpace(ConfirmPassword)
        && SelectedRole != null;

    private void Register()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return;
        }

        var user = _authService.Register(
            Username, Password, FullName, Email,
            SelectedRole!.RoleId, out string error);

        if (user == null)
        {
            ErrorMessage = error;
            return;
        }

        MessageBox.Show(
            $"Tài khoản \"{Username}\" đã được tạo thành công.\nVui lòng đăng nhập.",
            "Đăng ký thành công",
            MessageBoxButton.OK,
            MessageBoxImage.None);

        NavigateToLogin();
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
            if (w is Views.Auth.RegisterWindow)
            {
                w.Close();
                return;
            }
        }
    }
}
