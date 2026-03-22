using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

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

    public void Delete(Guid id)
    {
        _uow.GoodsReceipts.DeleteById(id);
        _uow.Save();
    }
}