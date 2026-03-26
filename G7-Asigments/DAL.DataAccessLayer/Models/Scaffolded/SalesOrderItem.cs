using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class SalesOrderItem
{
    public Guid Id { get; set; }

    public Guid SoId { get; set; }

    public Guid ProductId { get; set; }

    public decimal QtyOrdered { get; set; }

    public decimal QtyAllocated { get; set; }

    public decimal QtyDelivered { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? DiscountPct { get; set; }

    public decimal? LineTotal { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<GoodsIssueItem> GoodsIssueItems { get; set; } = new List<GoodsIssueItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual SalesOrder So { get; set; } = null!;
}
