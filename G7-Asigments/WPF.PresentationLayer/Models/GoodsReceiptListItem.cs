using DAL.DataAccessLayer.Models;

namespace WPF.PresentationLayer.Models;

public class GoodsReceiptListItem
{
    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public string GrnNumber => GoodsReceipt.GrnNumber;
    public DateOnly ReceiptDate => GoodsReceipt.ReceiptDate;
    public decimal TotalAmount => GoodsReceipt.TotalAmount;
    public string? Notes => GoodsReceipt.Notes;
    public int ItemCount => GoodsReceipt.GoodsReceiptItems?.Count ?? 0;

    public string StatusText => GoodsReceipt.StatusId switch
    {
        1 => "Nháp",
        2 => "Đã duyệt",
        3 => "Đã duyệt",
        4 => "Từ chối",
        _ => GoodsReceipt.StatusId.ToString()
    };

    public bool IsApproveVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
}
