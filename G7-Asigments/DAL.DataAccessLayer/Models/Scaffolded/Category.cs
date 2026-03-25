using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;G7-Asigments/DAL.DataAccessLayer/Models/Scaffolded/Category.cs

public partial class Category
{
    public Guid Id { get; set; }

    public string? CategoryCode { get; set; }

    public string CategoryName { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
