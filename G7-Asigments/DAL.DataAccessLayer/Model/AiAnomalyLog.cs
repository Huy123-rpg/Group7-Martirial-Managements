using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class AiAnomalyLog
{
    public Guid Id { get; set; }

    public string AnomalyType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte SeverityId { get; set; }

    public decimal? AnomalyScore { get; set; }

    public string? RefType { get; set; }

    public Guid? RefId { get; set; }

    public string? RefTable { get; set; }

    public Guid? AffectedUserId { get; set; }

    public string Status { get; set; } = null!;

    public Guid? AssignedTo { get; set; }

    public string? ResolutionNote { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }

    public string? ModelVersion { get; set; }

    public DateTimeOffset DetectedAt { get; set; }

    public virtual User? AffectedUser { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual LkpAnomalySeverity Severity { get; set; } = null!;
}
