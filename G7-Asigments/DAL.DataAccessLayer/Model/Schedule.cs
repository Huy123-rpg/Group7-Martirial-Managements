using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class Schedule
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public byte ScheduleType { get; set; }

    public string StatusCode { get; set; } = null!;

    public string? RefType { get; set; }

    public Guid? RefId { get; set; }

    public Guid? WarehouseId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public bool IsRecurring { get; set; }

    public string? RecurrenceRule { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? AssignedTo { get; set; }

    public int? ReminderMinutes { get; set; }

    public string? Description { get; set; }

    public string? Metadata { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual LkpScheduleType ScheduleTypeNavigation { get; set; } = null!;

    public virtual Warehouse? Warehouse { get; set; }
}
