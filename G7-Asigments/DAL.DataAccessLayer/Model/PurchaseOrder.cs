using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class PurchaseOrder
{
    public Guid Id { get; set; }

    public string PoNumber { get; set; } = null!;

    public Guid SupplierId { get; set; }

    public Guid WarehouseId { get; set; }

    public byte StatusId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly ExpectedDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }

    public string? PaymentTerms { get; set; }

    public string? Source { get; set; }

    public Guid? AiSuggestionId { get; set; }

    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<AiReorderSuggestion> AiReorderSuggestions { get; set; } = new List<AiReorderSuggestion>();

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
