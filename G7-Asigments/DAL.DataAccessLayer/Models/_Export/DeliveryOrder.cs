using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Export;

public class DeliveryOrder
{
    public Guid Id { get; set; }
    public string DoNumber { get; set; } = null!;
    public Guid GiId { get; set; }
    public Guid CustomerId { get; set; }
    public byte StatusId { get; set; }
    public string? ShippingAddress { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public DateTime? ActualDelivery { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual GoodsIssue GoodsIssue { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
}
