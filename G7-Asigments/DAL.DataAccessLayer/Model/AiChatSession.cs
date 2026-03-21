using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class AiChatSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? SessionTitle { get; set; }

    public string Messages { get; set; } = null!;

    public string? Context { get; set; }

    public int? TokenCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
