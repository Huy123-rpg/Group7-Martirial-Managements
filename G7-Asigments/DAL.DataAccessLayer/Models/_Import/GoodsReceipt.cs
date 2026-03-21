using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Import;

public class GoodsReceipt
{
    public Guid Id { get; set; }
    public string GrnNumber { get; set; } = null!;
    public Guid? PoId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public byte StatusId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public Guid? ValidatedBy { get; set; }
    public DateTimeOffset? ValidatedAt { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string? SupplierRef { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual PurchaseOrder? PurchaseOrder { get; set; }
    public virtual ICollection<GoodsReceiptItem> Items { get; set; } = [];
}
