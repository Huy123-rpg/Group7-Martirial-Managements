using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Export;

public class GoodsIssueItem
{
    public Guid Id { get; set; }
    public Guid GiId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ZoneId { get; set; }
    public decimal QtyIssued { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }  // computed
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }

    public virtual GoodsIssue GoodsIssue { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual WarehouseZone? Zone { get; set; }
}
