using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class UnitsOfMeasure
{
    public Guid Id { get; set; }

    public string UomCode { get; set; } = null!;

    public string UomName { get; set; } = null!;

    public Guid? BaseUomId { get; set; }

    public decimal? ConversionFactor { get; set; }

    public virtual UnitsOfMeasure? BaseUom { get; set; }

    public virtual ICollection<UnitsOfMeasure> InverseBaseUom { get; set; } = new List<UnitsOfMeasure>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
