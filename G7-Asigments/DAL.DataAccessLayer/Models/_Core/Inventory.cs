namespace DAL.DataAccessLayer.Models._Core;

public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? ZoneId { get; set; }
    public decimal QtyOnHand { get; set; }
    public decimal QtyReserved { get; set; }
    public decimal QtyAvailable { get; set; }  // computed: OnHand - Reserved
    public decimal QtyIncoming { get; set; }
    public decimal AvgCost { get; set; }
    public decimal TotalValue { get; set; }    // computed: OnHand * AvgCost
    public DateTimeOffset LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual WarehouseZone? Zone { get; set; }
}
