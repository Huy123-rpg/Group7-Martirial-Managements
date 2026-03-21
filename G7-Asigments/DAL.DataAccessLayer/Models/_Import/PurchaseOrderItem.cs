using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Import;

public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public Guid PoId { get; set; }
    public Guid ProductId { get; set; }
    public decimal QtyOrdered { get; set; }
    public decimal QtyReceived { get; set; }
    public decimal QtyRejected { get; set; }
    public decimal UnitCost { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal LineTotal { get; set; }  // computed
    public string? Notes { get; set; }

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
