namespace WPF.PresentationLayer.Helpers;

public static class PermissionHelper
{
    private static byte RoleId => SessionManager.CurrentUser?.RoleId ?? 0;

    public static bool IsAdmin => RoleId == 1;
    public static bool IsManager => RoleId == 2;
    public static bool IsStaff => RoleId == 3;
    public static bool IsAccountant => RoleId == 4;

    public static bool CanViewUsers => IsAdmin;
    public static bool CanViewDashboard => IsAdmin || IsManager || IsAccountant;

    // Purchase Order
    public static bool CanViewPurchaseOrder => IsAdmin || IsManager || IsStaff || IsAccountant;
    public static bool CanCreatePurchaseOrder => IsAdmin || IsManager || IsStaff;
    public static bool CanEditPurchaseOrder => IsAdmin || IsManager || IsStaff;
    public static bool CanDeletePurchaseOrder => IsAdmin || IsManager;
    public static bool CanApprovePurchaseOrder => IsAdmin || IsManager;

    // Sales Order
    public static bool CanViewSalesOrder => IsAdmin || IsManager || IsStaff || IsAccountant;
    public static bool CanCreateSalesOrder => IsAdmin || IsManager || IsStaff;
    public static bool CanEditSalesOrder => IsAdmin || IsManager || IsStaff;
    public static bool CanDeleteSalesOrder => IsAdmin || IsManager;
    public static bool CanApproveSalesOrder => IsAdmin || IsManager;

    // Goods Receipt
    public static bool CanViewGoodsReceipt => IsAdmin || IsManager || IsStaff || IsAccountant;
    public static bool CanCreateGoodsReceipt => IsAdmin || IsManager || IsStaff;
    public static bool CanEditGoodsReceipt => IsAdmin || IsManager || IsStaff;
    public static bool CanDeleteGoodsReceipt => IsAdmin || IsManager;
    public static bool CanApproveGoodsReceipt => IsAdmin || IsManager || IsAccountant;

    // Goods Issue
    public static bool CanViewGoodsIssue => IsAdmin || IsManager || IsStaff || IsAccountant;
    public static bool CanCreateGoodsIssue => IsAdmin || IsManager || IsStaff;
    public static bool CanEditGoodsIssue => IsAdmin || IsManager || IsStaff;
    public static bool CanDeleteGoodsIssue => IsAdmin || IsManager;
    public static bool CanApproveGoodsIssue => IsAdmin || IsManager || IsAccountant;
}
