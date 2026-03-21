using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class VExpiringSoon
{
    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public string? ZoneCode { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? DaysUntilExpiry { get; set; }

    public decimal? QtyAtRisk { get; set; }

    public decimal? ValueAtRisk { get; set; }
}
