namespace DAL.DataAccessLayer.Models._Core;

public class UnitOfMeasure
{
    public Guid Id { get; set; }
    public string UomCode { get; set; } = null!;
    public string UomName { get; set; } = null!;
    public Guid? BaseUomId { get; set; }
    public decimal? ConversionFactor { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];
}
