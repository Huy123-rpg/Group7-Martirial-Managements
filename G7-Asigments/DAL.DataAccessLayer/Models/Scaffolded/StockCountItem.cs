using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.DataAccessLayer.Models;

public partial class StockCountItem
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ZoneId { get; set; }

    public string? BatchNumber { get; set; }

    public decimal QtySystem { get; set; }

    public decimal? QtyCounted { get; set; }

    [NotMapped]
    public decimal QtyAvailable { get; set; }

    public decimal? QtyVariance { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? VarianceValue { get; set; }

    public DateTimeOffset? CountedAt { get; set; }

    public Guid? CountedBy { get; set; }

    public string? Notes { get; set; }

    public virtual User? CountedByNavigation { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual StockCountSession Session { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
