namespace WPF.PresentationLayer.Helpers;

public static class PermissionHelper
{
    private static byte RoleId => SessionManager.CurrentUser?.RoleId ?? 0;

    public static bool IsAdmin => RoleId == 1;
    public static bool IsManager => RoleId == 2;
    public static bool IsStaff => RoleId == 3;
    public static bool IsAccountant => RoleId == 4;

    public static bool CanViewUsers =>
        IsAdmin;

    public static bool CanViewDashboard =>
        IsAdmin || IsManager || IsAccountant;

    public static bool CanViewGoodsIssue =>
        IsAdmin || IsManager || IsStaff || IsAccountant;

    public static bool CanCreateGoodsIssue =>
        IsAdmin || IsManager || IsStaff;

    public static bool CanEditGoodsIssue =>
        IsAdmin || IsManager || IsStaff;

    public static bool CanDeleteGoodsIssue =>
        IsAdmin || IsManager;

    /// <summary>Chỉ Admin mới được duyệt phiếu xuất</summary>
    public static bool CanApproveGoodsIssue =>
        IsAdmin;

    public static bool CanViewGoodsReceipt =>
        IsAdmin || IsManager || IsStaff || IsAccountant;

    public static bool CanCreateGoodsReceipt =>
        IsAdmin || IsManager || IsStaff;

    public static bool CanEditGoodsReceipt =>
        IsAdmin || IsManager || IsStaff;

    public static bool CanDeleteGoodsReceipt =>
        IsAdmin || IsManager;

    /// <summary>Chỉ Admin mới được duyệt phiếu nhập</summary>
    public static bool CanApproveGoodsReceipt =>
        IsAdmin;

    public static bool CanViewPurchaseOrder =>
        IsAdmin || IsManager || IsStaff || IsAccountant;

    public static bool CanCreatePurchaseOrder =>
        IsAdmin || IsManager || IsStaff;
}