using BLL.BusinessLogicLayer.Services.Auth;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Auth;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService = new AuthService();

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Username { get => _username; set => SetField(ref _username, value); }
    public string Password { get => _password; set => SetField(ref _password, value); }
    public string ErrorMessage { get => _errorMessage; set => SetField(ref _errorMessage, value); }

    public RelayCommand LoginCommand => new(Login, CanLogin);

    private bool CanLogin() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

    private void Login()
    {
        var user = _authService.Login(Username, Password);
        if (user == null)
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return;
        }
        SessionManager.Login(user);
        ErrorMessage = string.Empty;
        // TODO: mở MainWindow và đóng LoginWindow
    }
}
