using DAL.DataAccessLayer.Models;

namespace BLL.BusinessLogicLayer.Services.Export;

public interface IGoodsIssueService
{
    IEnumerable<GoodsIssue> GetAll();
    IEnumerable<GoodsIssue> Search(string keyword);
    GoodsIssue? GetById(Guid id);
    void Create(GoodsIssue gi);
    void Update(GoodsIssue gi);
    void Delete(Guid id);
    void Approve(Guid id, Guid approvedBy);
}
