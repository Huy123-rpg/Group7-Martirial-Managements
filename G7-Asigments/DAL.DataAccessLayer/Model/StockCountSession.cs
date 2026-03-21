using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class StockCountSession
{
    public Guid Id { get; set; }

    public string SessionCode { get; set; } = null!;

    public Guid WarehouseId { get; set; }

    public Guid? ZoneId { get; set; }

    public byte StatusId { get; set; }

    public string? CountType { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? AssignedTo { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateOnly PlannedDate { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockCountItem> StockCountItems { get; set; } = new List<StockCountItem>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
