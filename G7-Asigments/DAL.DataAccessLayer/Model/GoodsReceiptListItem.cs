using DAL.DataAccessLayer.Model;

namespace WPF.PresentationLayer.Models;

public class GoodsReceiptListItem
{
    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public string GrnNumber => GoodsReceipt.GrnNumber;
    public DateOnly ReceiptDate => GoodsReceipt.ReceiptDate;
    public decimal TotalAmount => GoodsReceipt.TotalAmount;
    public string? Notes => GoodsReceipt.Notes;
    public int ItemCount => GoodsReceipt.GoodsReceiptItems?.Count ?? 0;

    // Được set từ code-behind sau khi biết role người dùng
    public bool IsApproveVisible { get; set; } = false;
}