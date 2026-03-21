namespace DAL.DataAccessLayer.Models._Core;

public class WarehouseZone
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string ZoneCode { get; set; } = null!;
    public string? ZoneName { get; set; }
    public byte ZoneType { get; set; }
    public decimal? CapacityM3 { get; set; }
    public int? CapacityPallet { get; set; }
    public bool IsActive { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
