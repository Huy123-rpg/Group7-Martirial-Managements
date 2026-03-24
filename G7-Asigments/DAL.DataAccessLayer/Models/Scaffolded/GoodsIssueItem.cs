using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class GoodsIssueItem
{
    public Guid Id { get; set; }

    public Guid GiId { get; set; }

    public Guid? SoItemId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ZoneId { get; set; }

    public decimal QtyRequested { get; set; }

    public decimal QtyIssued { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public string? Notes { get; set; }

    public virtual GoodsIssue Gi { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesOrderItem? SoItem { get; set; }

    public virtual WarehouseZone? Zone { get; set; }
}
