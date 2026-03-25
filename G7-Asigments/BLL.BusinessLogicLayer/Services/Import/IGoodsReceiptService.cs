using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Import;

public interface IGoodsReceiptService
{
    IEnumerable<GoodsReceipt> GetAll();
    GoodsReceipt? GetById(Guid id);
    void Create(GoodsReceipt receipt);
    void Update(GoodsReceipt receipt);
    void Delete(Guid id);
    IEnumerable<GoodsReceipt> Search(string keyword);
}