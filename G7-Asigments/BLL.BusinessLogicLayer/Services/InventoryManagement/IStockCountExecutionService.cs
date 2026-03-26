using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.InventoryManagement;

public interface IStockCountExecutionService
{
    IEnumerable<StockCountSession> GetAssignedSessions(Guid staffId);
    IEnumerable<StockCountItem> GetSessionItems(Guid sessionId);
    void SaveCount(Guid sessionId, IEnumerable<StockCountItem> items, bool isComplete, Guid staffId, string? notes = null);
}
