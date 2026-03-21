using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class LkpAnomalySeverity
{
    public byte SeverityId { get; set; }

    public string SeverityCode { get; set; } = null!;

    public string SeverityName { get; set; } = null!;

    public virtual ICollection<AiAnomalyLog> AiAnomalyLogs { get; set; } = new List<AiAnomalyLog>();
}
