using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Import;

public interface IGoodsReceiptService
{
    IEnumerable<GoodsReceipt> GetAll();
    IEnumerable<GoodsReceipt> Search(string keyword);
    GoodsReceipt? GetById(Guid id);
    void Create(GoodsReceipt gr);
    void Update(GoodsReceipt gr);
    void Delete(Guid id);
    void Approve(Guid id, Guid approvedBy);
    void Cancel(Guid id, Guid cancelledBy, string reason);
}
