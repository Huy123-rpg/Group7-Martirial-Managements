using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.Export;

public interface ISalesOrderService
{
    IEnumerable<SalesOrder> GetAll();
    IEnumerable<SalesOrder> Search(string keyword);
    SalesOrder? GetById(Guid id);
    void Create(SalesOrder so);
    void Update(SalesOrder so);
    void Delete(Guid id);
    void Approve(Guid id, Guid approvedBy);
    void Reject(Guid id, string reason);
}
