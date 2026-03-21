using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class VPendingPoReceipt
{
    public string PoNumber { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public DateOnly ExpectedDate { get; set; }

    public int? DaysOverdue { get; set; }

    public string Sku { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public decimal QtyOrdered { get; set; }

    public decimal QtyReceived { get; set; }

    public decimal? QtyOutstanding { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? OutstandingValue { get; set; }
}
