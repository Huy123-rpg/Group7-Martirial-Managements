using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Model;
using DAL.DataAccessLayer.Context;

namespace BLL.BusinessLogicLayer.Services.Export;

public class GoodsIssueService : IGoodsIssueService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public IEnumerable<GoodsIssue> GetAll() =>
        _uow.GoodsIssues.GetAll().OrderByDescending(g => g.IssueDate);

    public IEnumerable<GoodsIssue> Search(string keyword) =>
        _uow.GoodsIssues.Find(g =>
            g.GiNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public GoodsIssue? GetById(Guid id) => _uow.GoodsIssues.GetById(id);

    public void Create(GoodsIssue gi)
    {
        gi.Id = Guid.NewGuid();
        gi.CreatedAt = DateTimeOffset.UtcNow;
        gi.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Add(gi);
        _uow.Save();
    }

    public void Update(GoodsIssue gi)
    {
        gi.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Update(gi);
        _uow.Save();
    }

    public void Delete(Guid id)
    {
        // 1. Xoá hết các dòng chi tiết trước để tránh lỗi khoá ngoại (FK)
        var items = _uow.GoodsIssueItems.GetAll().Where(x => x.GiId == id).ToList();
        foreach (var item in items)
        {
            _uow.GoodsIssueItems.DeleteById(item.Id);
        }

        // 2. Sau đó mới xoá phiếu xuất
        _uow.GoodsIssues.DeleteById(id);
        _uow.Save();
    }

    public void Approve(Guid id, Guid approvedBy)
    {
        var gi = _uow.GoodsIssues.GetById(id) ?? throw new Exception("GI not found");
        gi.StatusId = 3;
        gi.ApprovedBy = approvedBy;
        gi.ApprovedAt = DateTimeOffset.UtcNow;
        gi.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.GoodsIssues.Update(gi);
        _uow.Save();
    }
}
