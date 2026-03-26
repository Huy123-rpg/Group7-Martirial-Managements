using DAL.DataAccessLayer.Models;

namespace WPF.PresentationLayer.Models;

public class GoodsIssueListItem
{
    public GoodsIssue GoodsIssue { get; set; } = null!;
    public string GiNumber => GoodsIssue.GiNumber;
    public DateOnly IssueDate => GoodsIssue.IssueDate;
    public decimal TotalAmount => GoodsIssue.TotalAmount;
    public string? Notes => GoodsIssue.Notes;
    public int ItemCount => GoodsIssue.GoodsIssueItems?.Count ?? 0;

    public string StatusText => GoodsIssue.StatusId switch
    {
        1 => "Nháp",
        2 => "Đã duyệt",
        3 => "Đã duyệt",
        4 => "Từ chối",
        _ => GoodsIssue.StatusId.ToString()
    };

    public bool IsApproveVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
}
