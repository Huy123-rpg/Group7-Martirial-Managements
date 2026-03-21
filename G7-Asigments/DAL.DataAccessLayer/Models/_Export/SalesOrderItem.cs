using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Export;

public class SalesOrderItem
{
    public Guid Id { get; set; }
    public Guid SoId { get; set; }
    public Guid ProductId { get; set; }
    public decimal QtyOrdered { get; set; }
    public decimal QtyIssued { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPct { get; set; }
    public decimal LineTotal { get; set; }  // computed
    public string? Notes { get; set; }

    public virtual SalesOrder SalesOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
