using DAL.DataAccessLayer.Models;

namespace WPF.PresentationLayer.Models;

public class PurchaseOrderListItem
{
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public string PoNumber => PurchaseOrder.PoNumber;
    public string SupplierName => PurchaseOrder.Supplier?.SupplierName ?? "";
    public string WarehouseName => PurchaseOrder.Warehouse?.Name ?? "";
    public DateOnly OrderDate => PurchaseOrder.OrderDate;
    public DateOnly ExpectedDate => PurchaseOrder.ExpectedDate;
    public decimal TotalAmount => PurchaseOrder.TotalAmount;
    public string StatusText => PurchaseOrder.StatusId switch
    {
        1 => "Nháp",
        2 => "Chờ duyệt",
        3 => "Đã duyệt",
        4 => "Từ chối",
        5 => "Đã hủy",
        _ => PurchaseOrder.StatusId.ToString()
    };

    public bool IsApproveVisible { get; set; }
    public bool IsCreateReceiptVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
    public bool IsSubmitVisible { get; set; }
    public bool IsCancelVisible { get; set; }
}
