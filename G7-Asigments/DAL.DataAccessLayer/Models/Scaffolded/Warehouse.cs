using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class Warehouse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? City { get; set; }

    public Guid? ManagerId { get; set; }

    public bool IsActive { get; set; }

    public byte CostingMethod { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<AiForecast> AiForecasts { get; set; } = new List<AiForecast>();

    public virtual ICollection<AiReorderSuggestion> AiReorderSuggestions { get; set; } = new List<AiReorderSuggestion>();

    public virtual LkpCostingMethod CostingMethodNavigation { get; set; } = null!;

    public virtual ICollection<GoodsIssue> GoodsIssues { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual User? Manager { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockCountSession> StockCountSessions { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<StockTransfer> StockTransferFromWarehouses { get; set; } = new List<StockTransfer>();

    public virtual ICollection<StockTransfer> StockTransferToWarehouses { get; set; } = new List<StockTransfer>();

    public virtual ICollection<WarehouseZone> WarehouseZones { get; set; } = new List<WarehouseZone>();
}
