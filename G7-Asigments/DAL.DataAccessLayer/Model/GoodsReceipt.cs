using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class GoodsReceipt
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

    public DateOnly ReceiptDate { get; set; }

    public string? SupplierRef { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();

    public virtual PurchaseOrder? Po { get; set; }

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User? ValidatedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
