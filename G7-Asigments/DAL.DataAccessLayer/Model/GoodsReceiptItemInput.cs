namespace WPF.PresentationLayer.Models;

public class GoodsReceiptItemInput
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal QtyReceived { get; set; }
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }

    public decimal LineTotal => QtyReceived * UnitCost;
}