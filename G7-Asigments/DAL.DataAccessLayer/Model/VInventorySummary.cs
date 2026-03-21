using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class VInventorySummary
{
    public Guid Id { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public string? ZoneCode { get; set; }

    public string? ZoneType { get; set; }

    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? CategoryName { get; set; }

    public string? UomCode { get; set; }

    public decimal QtyOnHand { get; set; }

    public decimal QtyReserved { get; set; }

    public decimal? QtyAvailable { get; set; }

    public decimal QtyIncoming { get; set; }

    public decimal AvgCost { get; set; }

    public decimal? TotalValue { get; set; }

    public string StockStatus { get; set; } = null!;

    public DateTimeOffset LastUpdated { get; set; }
}
