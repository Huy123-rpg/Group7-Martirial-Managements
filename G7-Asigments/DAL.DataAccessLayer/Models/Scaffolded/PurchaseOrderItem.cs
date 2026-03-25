using System;
using System.Collections.Generic;

<<<<<<<< HEAD:G7-Asigments/DAL.DataAccessLayer/Model/PurchaseOrderItem.cs
namespace DAL.DataAccessLayer.Model;
========
namespace DAL.DataAccessLayer.Models;
>>>>>>>> origin/main:G7-Asigments/DAL.DataAccessLayer/Models/Scaffolded/PurchaseOrderItem.cs

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
