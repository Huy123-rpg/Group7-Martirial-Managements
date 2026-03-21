namespace DAL.DataAccessLayer.Models._Core;

public class Warehouse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; }
    public byte CostingMethod { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<WarehouseZone> Zones { get; set; } = [];
}
