using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Export;

public class GoodsIssue
{
    public Guid Id { get; set; }
    public string GiNumber { get; set; } = null!;
    public Guid? SoId { get; set; }
    public Guid WarehouseId { get; set; }
    public byte StatusId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual SalesOrder? SalesOrder { get; set; }
    public virtual ICollection<GoodsIssueItem> Items { get; set; } = [];
    public virtual DeliveryOrder? DeliveryOrder { get; set; }
}
