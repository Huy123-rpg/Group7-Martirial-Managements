using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class StockAdjustment
{
    public Guid Id { get; set; }

    public string AdjNumber { get; set; } = null!;

    public Guid? SessionId { get; set; }

    public Guid WarehouseId { get; set; }

    public byte StatusId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public Guid? ValidatedBy { get; set; }

    public DateTimeOffset? ValidatedAt { get; set; }

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual StockCountSession? Session { get; set; }

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual ICollection<StockAdjustmentItem> StockAdjustmentItems { get; set; } = new List<StockAdjustmentItem>();

    public virtual User? ValidatedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
