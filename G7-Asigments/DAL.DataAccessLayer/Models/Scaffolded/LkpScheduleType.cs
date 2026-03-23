using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class LkpScheduleType
{
    public byte TypeId { get; set; }

    public string TypeCode { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
