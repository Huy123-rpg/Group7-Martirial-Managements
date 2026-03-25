using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class StockTransferItem
{
    public Guid Id { get; set; }

    public Guid TransferId { get; set; }

    public Guid ProductId { get; set; }

    public string? BatchNumber { get; set; }

    public decimal QtyRequested { get; set; }

    public decimal QtyTransferred { get; set; }

    public decimal UnitCost { get; set; }

    public string? Notes { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual StockTransfer Transfer { get; set; } = null!;
}
