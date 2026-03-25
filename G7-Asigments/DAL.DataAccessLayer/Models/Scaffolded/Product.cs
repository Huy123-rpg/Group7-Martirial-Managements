using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public string ProductName { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public Guid? UomId { get; set; }

    public decimal ReorderPoint { get; set; }

    public decimal MinStock { get; set; }

    public decimal? MaxStock { get; set; }

    public decimal SafetyStock { get; set; }

    public decimal? WeightKg { get; set; }

    public decimal? VolumeM3 { get; set; }

    public int? ShelfLifeDays { get; set; }

    public bool IsBatchTracked { get; set; }

    public bool IsExpiryTracked { get; set; }

    public decimal? StandardCost { get; set; }

    public string? Attributes { get; set; }

    public string? ImageUrl { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<AiForecast> AiForecasts { get; set; } = new List<AiForecast>();

    public virtual ICollection<AiReorderSuggestion> AiReorderSuggestions { get; set; } = new List<AiReorderSuggestion>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<GoodsIssueItem> GoodsIssueItems { get; set; } = new List<GoodsIssueItem>();

    public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();

    public virtual ICollection<StockAdjustmentItem> StockAdjustmentItems { get; set; } = new List<StockAdjustmentItem>();

    public virtual ICollection<StockCountItem> StockCountItems { get; set; } = new List<StockCountItem>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    public virtual UnitsOfMeasure? Uom { get; set; }
}
