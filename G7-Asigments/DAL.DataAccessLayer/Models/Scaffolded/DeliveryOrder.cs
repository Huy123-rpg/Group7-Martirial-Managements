using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class DeliveryOrder
{
    public Guid Id { get; set; }

    public string DoNumber { get; set; } = null!;

    public Guid GiId { get; set; }

    public Guid? SoId { get; set; }

    public Guid? CustomerId { get; set; }

    public byte StatusId { get; set; }

    public Guid? AssignedTo { get; set; }

    public string? VehicleInfo { get; set; }

    public string? CarrierName { get; set; }

    public string? TrackingNumber { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? ContactPhone { get; set; }

    public DateOnly PlannedDate { get; set; }

    public DateOnly? ActualDate { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public string? RecipientName { get; set; }

    public string? DeliveryProof { get; set; }

    public string? Notes { get; set; }

    public string? FailureReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual GoodsIssue Gi { get; set; } = null!;

    public virtual SalesOrder? So { get; set; }

    public virtual LkpDeliveryStatus Status { get; set; } = null!;
}
