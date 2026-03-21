using DAL.DataAccessLayer.Models._Core;

namespace BLL.BusinessLogicLayer.Services.Scheduling;

public interface IScheduleService
{
    IEnumerable<Schedule> GetAll();
    IEnumerable<Schedule> GetByWarehouse(Guid warehouseId);
    IEnumerable<Schedule> Search(string keyword);
    Schedule? GetById(Guid id);
    void Create(Schedule schedule);
    void Update(Schedule schedule);
    void Delete(Guid id);
    IEnumerable<Schedule> GetByDateRange(DateTime from, DateTime to);
}
