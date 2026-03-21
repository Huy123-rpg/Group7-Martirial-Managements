using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Scheduling;

public class ScheduleService : IScheduleService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<Schedule> GetAll() =>
        _uow.Schedules.GetAll().OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> GetByWarehouse(Guid warehouseId) =>
        _uow.Schedules.Find(s => s.WarehouseId == warehouseId)
                      .OrderBy(s => s.StartTime);

    public IEnumerable<Schedule> Search(string keyword) =>
        _uow.Schedules.Find(s =>
            s.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (s.Description ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public Schedule? GetById(Guid id) => _uow.Schedules.GetById(id);

    public void Create(Schedule schedule)
    {
        schedule.Id = Guid.NewGuid();
        schedule.CreatedAt = DateTimeOffset.UtcNow;
        schedule.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Schedules.Add(schedule);
        _uow.Save();
    }

    public void Update(Schedule schedule)
    {
        schedule.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Schedules.Update(schedule);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        _uow.Schedules.DeleteById(id);
        _uow.Save();
    }

    public IEnumerable<Schedule> GetByDateRange(DateTime from, DateTime to)
    {
        var fromOffset = new DateTimeOffset(from);
        var toOffset = new DateTimeOffset(to);

        return _uow.Schedules.Find(s => s.StartTime >= fromOffset && s.StartTime <= toOffset)
                             .OrderBy(s => s.StartTime);
    }
}