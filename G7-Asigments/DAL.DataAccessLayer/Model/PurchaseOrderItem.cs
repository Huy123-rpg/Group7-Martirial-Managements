using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class PurchaseOrderItem
{
    public Guid Id { get; set; }

    public Guid PoId { get; set; }

    public Guid ProductId { get; set; }

    public decimal QtyOrdered { get; set; }

    public decimal QtyReceived { get; set; }

    public decimal QtyRejected { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? TaxRate { get; set; }

    public decimal? LineTotal { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();

    public virtual PurchaseOrder Po { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
