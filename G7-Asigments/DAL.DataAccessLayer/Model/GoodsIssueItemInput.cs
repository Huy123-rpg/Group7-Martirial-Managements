namespace WPF.PresentationLayer.Models;

public class GoodsIssueItemInput
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal QtyIssued { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }

    public decimal LineTotal => QtyIssued * UnitPrice;
}