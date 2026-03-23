using DAL.DataAccessLayer.Models;

namespace WPF.PresentationLayer.Helpers;

public static class SessionManager
{
    public static User? CurrentUser { get; private set; }
    public static string CurrentRole => CurrentUser?.Role?.RoleCode ?? string.Empty;

    public static bool IsAdmin => CurrentRole == "ADMIN";
    public static bool IsManager => CurrentRole == "MANAGER";
    public static bool IsStaff => CurrentRole == "STAFF";
    public static bool IsAccountant => CurrentRole == "ACCOUNTANT";
    public static bool IsSupplier => CurrentRole == "SUPPLIER";

    public static void Login(User user) => CurrentUser = user;
    public static void Logout() => CurrentUser = null;
    public static bool IsLoggedIn => CurrentUser != null;
}
