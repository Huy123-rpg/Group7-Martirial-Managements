using DAL.DataAccessLayer.Models._Lookup;

namespace DAL.DataAccessLayer.Models._Core;

public class Schedule
{
    public Guid Id { get; set; }
    public byte ScheduleTypeId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid? CreatedById { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public byte StatusId { get; set; }
    public Guid? RefDocumentId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual User? AssignedTo { get; set; }
    public virtual LkpScheduleType ScheduleType { get; set; } = null!;
}
