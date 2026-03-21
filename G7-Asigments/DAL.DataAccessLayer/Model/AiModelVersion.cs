using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class AiModelVersion
{
    public Guid Id { get; set; }

    public string ModelName { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? AccuracyScore { get; set; }

    public decimal? MapeScore { get; set; }

    public DateOnly? TrainingDate { get; set; }

    public int? TrainingRecords { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset? DeployedAt { get; set; }

    public DateTimeOffset? DeprecatedAt { get; set; }

    public string? Config { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
