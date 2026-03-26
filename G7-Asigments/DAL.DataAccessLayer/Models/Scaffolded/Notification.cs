using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Channel { get; set; } = null!;

    public string? RefType { get; set; }

    public Guid? RefId { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
