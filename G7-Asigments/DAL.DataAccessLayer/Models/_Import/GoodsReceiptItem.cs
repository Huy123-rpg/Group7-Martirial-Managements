using DAL.DataAccessLayer.Models._Core;

namespace DAL.DataAccessLayer.Models._Import;

public class GoodsReceiptItem
{
    public Guid Id { get; set; }
    public Guid GrnId { get; set; }
    public Guid? PoItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ZoneId { get; set; }
    public decimal QtyReceived { get; set; }
    public decimal QtyAccepted { get; set; }
    public decimal QtyRejected { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }  // computed
    public string? Notes { get; set; }

    public virtual GoodsReceipt GoodsReceipt { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual WarehouseZone? Zone { get; set; }
}
