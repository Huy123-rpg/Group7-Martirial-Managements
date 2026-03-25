using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public Guid? RecordId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? ChangedFields { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? RequestId { get; set; }

    public DateTimeOffset LoggedAt { get; set; }

    public virtual User? User { get; set; }
}
