using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class AiReorderSuggestion
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? SupplierId { get; set; }

    public decimal SuggestedQty { get; set; }

    public DateOnly? SuggestedDate { get; set; }

    public string TriggerReason { get; set; } = null!;

    public decimal QtyOnHand { get; set; }

    public decimal ReorderPoint { get; set; }

    public decimal? DaysOfStock { get; set; }

    public string Status { get; set; } = null!;

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public Guid? PoId { get; set; }

    public string? RejectionNote { get; set; }

    public string? ModelVersion { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual PurchaseOrder? Po { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier? Supplier { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
