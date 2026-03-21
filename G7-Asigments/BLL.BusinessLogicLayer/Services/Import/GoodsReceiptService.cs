using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models._Import;

namespace BLL.BusinessLogicLayer.Services.Import;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<GoodsReceipt> GetAll() =>
        _uow.GoodsReceipts.GetAll().OrderByDescending(g => g.ReceiptDate);

    public IEnumerable<GoodsReceipt> Search(string keyword) =>
        _uow.GoodsReceipts.Find(g =>
            g.GrnNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public GoodsReceipt? GetById(Guid id) => _uow.GoodsReceipts.GetById(id);

    public void Create(GoodsReceipt gr)
    {
        gr.Id = Guid.NewGuid();
        gr.CreatedAt = DateTimeOffset.UtcNow;
        gr.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsReceipts.Add(gr);
        _uow.Save();
    }

    public void Update(GoodsReceipt gr)
    {
        gr.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsReceipts.Update(gr);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
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
}
