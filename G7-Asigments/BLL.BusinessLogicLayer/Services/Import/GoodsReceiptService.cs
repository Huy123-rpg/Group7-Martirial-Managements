using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.Import;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<GoodsReceipt> GetAll() =>
        _uow.GoodsReceipts.GetAll().OrderByDescending(x => x.ReceiptDate);

    public GoodsReceipt? GetById(Guid id) =>
        _uow.GoodsReceipts.GetById(id);

    public void Create(GoodsReceipt receipt)
    {
        _uow.GoodsReceipts.Add(receipt);
        _uow.Save();
    }

    public void Update(GoodsReceipt receipt)
    {
        _uow.GoodsReceipts.Update(receipt);
        _uow.Save();
    }

    public IEnumerable<GoodsReceipt> Search(string keyword) =>
        _uow.GoodsReceipts.Find(g =>
            g.GrnNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public void Delete(Guid id)
    {
        // 1. Xoá hết các dòng chi tiết trước để tránh lỗi khoá ngoại (FK)
        var items = _uow.GoodsReceiptItems.GetAll().Where(x => x.GrnId == id).ToList();
        foreach (var item in items)
        {
            _uow.GoodsReceiptItems.DeleteById(item.Id);
        }

        // 2. Sau đó mới xoá phiếu nhập
        _uow.GoodsReceipts.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var gr = _uow.GoodsReceipts.GetById(id) ?? throw new Exception("GR not found");
        gr.StatusId = 3;
        gr.ApprovedBy = approvedBy;
        gr.ApprovedAt = DateTimeOffset.UtcNow;
        gr.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsReceipts.Update(gr);
        _uow.Save();
    }

    public void Cancel(Guid id, Guid cancelledBy, string reason)
    {
        var gr = _uow.GoodsReceipts.GetById(id) ?? throw new Exception("GR not found");
        gr.StatusId = 4;
        gr.RejectionReason = reason;
        gr.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsReceipts.Update(gr);
        _uow.Save();
    }
}