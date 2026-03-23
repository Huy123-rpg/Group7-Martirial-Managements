using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class StockAdjustmentItem
{
    public Guid Id { get; set; }

    public Guid AdjId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ZoneId { get; set; }

    public string? BatchNumber { get; set; }

    public decimal QtyBefore { get; set; }

    public decimal QtyChange { get; set; }

    public decimal? QtyAfter { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? ValueChange { get; set; }

    public string? Notes { get; set; }

    public virtual StockAdjustment Adj { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
