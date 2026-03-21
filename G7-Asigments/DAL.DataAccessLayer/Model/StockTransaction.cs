using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class StockTransaction
{
    public Guid Id { get; set; }

    public string TxnCode { get; set; } = null!;

    public byte TxnTypeId { get; set; }

    public Guid ProductId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? ZoneId { get; set; }

    public decimal QtyChange { get; set; }

    public decimal QtyBalance { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? TotalValue { get; set; }

    public string? BatchNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? RefType { get; set; }

    public Guid? RefId { get; set; }

    public Guid? RefItemId { get; set; }

    public Guid PerformedBy { get; set; }

    public DateTimeOffset TxnAt { get; set; }

    public string? Note { get; set; }

    public virtual User PerformedByNavigation { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual LkpTransactionType TxnType { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
