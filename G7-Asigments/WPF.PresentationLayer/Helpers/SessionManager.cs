using DAL.DataAccessLayer.Model;

namespace WPF.PresentationLayer.Helpers;

public static class SessionManager
{
    public static User? CurrentUser { get; private set; }

    public static bool IsAdmin      => CurrentUser?.RoleId == 1;
    public static bool IsManager    => CurrentUser?.RoleId == 2;
    public static bool IsStaff      => CurrentUser?.RoleId == 3;
    public static bool IsAccountant => CurrentUser?.RoleId == 4;
    public static bool IsSupplier   => CurrentUser?.RoleId == 5;

    public static string CurrentRole => CurrentUser?.Role?.RoleCode ?? string.Empty;

    public static void Login(User user) => CurrentUser = user;
    public static void Logout() => CurrentUser = null;
    public static bool IsLoggedIn => CurrentUser != null;
}
