using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class LkpTransactionType
{
    public byte TypeId { get; set; }

    public string TypeCode { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
