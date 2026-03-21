using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class LkpCostingMethod
{
    public byte MethodId { get; set; }

    public string MethodCode { get; set; } = null!;

    public string MethodName { get; set; } = null!;

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
