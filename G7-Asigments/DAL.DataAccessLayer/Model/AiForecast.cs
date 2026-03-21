using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class AiForecast
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid WarehouseId { get; set; }

    public DateOnly ForecastDate { get; set; }

    public string ForecastPeriod { get; set; } = null!;

    public decimal PredictedDemand { get; set; }

    public decimal? PredictedMin { get; set; }

    public decimal? PredictedMax { get; set; }

    public decimal? ConfidenceScore { get; set; }

    public string ModelName { get; set; } = null!;

    public string ModelVersion { get; set; } = null!;

    public string? FeaturesUsed { get; set; }

    public decimal? ActualDemand { get; set; }

    public decimal? MapeError { get; set; }

    public DateTimeOffset GeneratedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
