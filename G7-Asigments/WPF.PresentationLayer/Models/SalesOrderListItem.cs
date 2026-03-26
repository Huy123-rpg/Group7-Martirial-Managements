using DAL.DataAccessLayer.Models;

namespace WPF.PresentationLayer.Models;

public class SalesOrderListItem
{
    public SalesOrder SalesOrder { get; set; } = null!;
    public string SoNumber => SalesOrder.SoNumber;
    public string CustomerName { get; set; } = "";
    public string WarehouseName { get; set; } = "";
    public DateOnly OrderDate => SalesOrder.OrderDate;
    public decimal TotalAmount => SalesOrder.TotalAmount;
    public string StatusText => SalesOrder.StatusId switch
    {
        1 => "Nháp",
        2 => "Chờ duyệt",
        3 => "Đã duyệt",
        4 => "Từ chối",
        5 => "Đã hủy",
        _ => SalesOrder.StatusId.ToString()
    };

    public bool IsSubmitVisible { get; set; }
    public bool IsApproveVisible { get; set; }
    public bool IsCreateIssueVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
}
