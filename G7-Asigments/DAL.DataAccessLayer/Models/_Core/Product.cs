namespace DAL.DataAccessLayer.Models._Core;

public class Product
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = null!;
    public string? Barcode { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid? CategoryId { get; set; }
    public Guid? UomId { get; set; }
    public decimal ReorderPoint { get; set; }
    public decimal MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public decimal SafetyStock { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? VolumeM3 { get; set; }
    public int? ShelfLifeDays { get; set; }
    public bool IsBatchTracked { get; set; }
    public bool IsExpiryTracked { get; set; }
    public decimal? StandardCost { get; set; }
    public string? ImageUrl { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }
    public virtual UnitOfMeasure? Uom { get; set; }
}
