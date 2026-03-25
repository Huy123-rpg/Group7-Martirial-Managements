using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.DataAccessLayer.Model;

public partial class Schedule
{
    [NotMapped]
    public DateTimeOffset? CompletedAt { get; set; }

    [NotMapped]
    public string? CancellationNote { get; set; }
}
