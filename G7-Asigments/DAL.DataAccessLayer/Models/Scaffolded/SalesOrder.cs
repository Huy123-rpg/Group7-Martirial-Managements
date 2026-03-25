using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class SalesOrder
{
    public Guid Id { get; set; }

    public string SoNumber { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid WarehouseId { get; set; }

    public byte StatusId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly RequiredDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }

    public string? PaymentTerms { get; set; }

    public string? ShippingAddress { get; set; }

    public string? Notes { get; set; }

    public string? RejectionReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();

    public virtual ICollection<GoodsIssue> GoodsIssues { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
