using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public interface IStockCountApprovalService
{
    // ─── Queries ─────────────────────────────────────────────────────────────
    IEnumerable<StockCountSession> GetAll();
    IEnumerable<StockCountSession> GetByManagerWarehouse(Guid managerId);

    // ─── Approval Actions (Manager) ───────────────────────────────────────────
    void Approve(Guid sessionId, Guid managerId);
    void Reject(Guid sessionId, Guid managerId, string reason);
}
