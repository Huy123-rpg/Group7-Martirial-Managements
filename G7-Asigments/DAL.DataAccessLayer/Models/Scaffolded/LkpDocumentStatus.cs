using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class LkpDocumentStatus
{
    public byte StatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<GoodsIssue> GoodsIssues { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockCountSession> StockCountSessions { get; set; } = new List<StockCountSession>();

    public virtual ICollection<StockTransfer> StockTransfers { get; set; } = new List<StockTransfer>();
}
