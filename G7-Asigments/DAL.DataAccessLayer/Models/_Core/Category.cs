namespace DAL.DataAccessLayer.Models._Core;

public class Category
{
    public Guid Id { get; set; }
    public string? CategoryCode { get; set; }
    public string CategoryName { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];
}
