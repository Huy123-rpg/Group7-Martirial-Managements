using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class WarehouseZone
{
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public string ZoneCode { get; set; } = null!;

    public string? ZoneName { get; set; }

    public byte ZoneType { get; set; }

    public decimal? CapacityM3 { get; set; }

    public int? CapacityPallet { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<GoodsIssueItem> GoodsIssueItems { get; set; } = new List<GoodsIssueItem>();

    public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<StockAdjustmentItem> StockAdjustmentItems { get; set; } = new List<StockAdjustmentItem>();

    public virtual ICollection<StockCountItem> StockCountItems { get; set; } = new List<StockCountItem>();

    public virtual ICollection<StockCountSession> StockCountSessions { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<StockTransfer> StockTransferFromZones { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferToZones { get; set; } = new List<StockTransfer>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual LkpZoneType ZoneTypeNavigation { get; set; } = null!;
}
