using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Import;

public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string PoNumber { get; set; } = null!;
    public Guid SupplierId { get; set; }
    public Guid WarehouseId { get; set; }
    public byte StatusId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = [];
}
