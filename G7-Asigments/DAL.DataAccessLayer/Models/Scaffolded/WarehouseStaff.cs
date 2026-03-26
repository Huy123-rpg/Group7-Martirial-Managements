using System;

namespace DAL.DataAccessLayer.Models;

public class WarehouseStaff
{
    public Guid WarehouseId { get; set; }
    public Guid UserId      { get; set; }
    public Guid AssignedBy  { get; set; }
    public DateTimeOffset AssignedAt { get; set; }

    public virtual Warehouse Warehouse          { get; set; } = null!;
    public virtual User      User               { get; set; } = null!;
    public virtual User      AssignedByNavigation { get; set; } = null!;
}
