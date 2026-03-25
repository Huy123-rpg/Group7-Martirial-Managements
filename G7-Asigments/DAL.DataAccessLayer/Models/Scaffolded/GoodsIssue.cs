using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class GoodsIssue
{
    public Guid Id { get; set; }

    public string GiNumber { get; set; } = null!;

    public Guid? SoId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid WarehouseId { get; set; }

    public byte StatusId { get; set; }

    public string? IssueType { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateOnly IssueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }

    public string? Reason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();

    public virtual ICollection<GoodsIssueItem> GoodsIssueItems { get; set; } = new List<GoodsIssueItem>();

    public virtual SalesOrder? So { get; set; }

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
