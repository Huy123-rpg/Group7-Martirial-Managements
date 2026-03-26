using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Import;

public interface IPurchaseOrderService
{
    IEnumerable<PurchaseOrder> GetAll();
    IEnumerable<PurchaseOrder> Search(string keyword);
    PurchaseOrder? GetById(Guid id);
    void Create(PurchaseOrder po);
    void Update(PurchaseOrder po);
    void Delete(Guid id);
    void Submit(Guid id);
    void Approve(Guid id, Guid approvedBy);
    void Reject(Guid id, string reason);
    void Cancel(Guid id, Guid cancelledBy);
}
