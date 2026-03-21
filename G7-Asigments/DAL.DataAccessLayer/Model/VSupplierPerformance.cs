using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class VSupplierPerformance
{
    public string SupplierCode { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public int? TotalPos { get; set; }

    public int? TotalGrns { get; set; }

    public double? AvgDelayDays { get; set; }

    public decimal? TotalQtyReceived { get; set; }

    public decimal? TotalQtyRejected { get; set; }

    public decimal? RejectionRatePct { get; set; }

    public decimal? TotalPurchaseValue { get; set; }
}
