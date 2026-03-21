using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class GoodsReceiptItem
{
    public Guid Id { get; set; }

    public Guid GrnId { get; set; }

    public Guid? PoItemId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ZoneId { get; set; }

    public decimal QtyReceived { get; set; }

    public decimal QtyAccepted { get; set; }

    public decimal QtyRejected { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? LineTotal { get; set; }

    public string? Notes { get; set; }

    public virtual GoodsReceipt Grn { get; set; } = null!;

    public virtual PurchaseOrderItem? PoItem { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
