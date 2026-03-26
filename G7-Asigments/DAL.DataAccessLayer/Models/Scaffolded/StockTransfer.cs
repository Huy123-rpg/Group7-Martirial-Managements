using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class StockTransfer
{
    public Guid Id { get; set; }

    public string TransferNumber { get; set; } = null!;

    public Guid FromWarehouseId { get; set; }

    public Guid? FromZoneId { get; set; }

    public Guid ToWarehouseId { get; set; }

    public Guid? ToZoneId { get; set; }

    public byte StatusId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateOnly TransferDate { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Warehouse FromWarehouse { get; set; } = null!;

    public virtual WarehouseZone? FromZone { get; set; }

    public virtual LkpDocumentStatus Status { get; set; } = null!;

    public virtual ICollection<StockTransferItem> StockTransferItems { get; set; } = new List<StockTransferItem>();

    public virtual Warehouse ToWarehouse { get; set; } = null!;

    public virtual WarehouseZone? ToZone { get; set; }
}
