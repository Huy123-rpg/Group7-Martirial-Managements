using System;
using System.Collections.Generic;

<<<<<<<< HEAD:G7-Asigments/DAL.DataAccessLayer/Model/Inventory.cs
namespace DAL.DataAccessLayer.Model;
========
namespace DAL.DataAccessLayer.Models;
>>>>>>>> origin/main:G7-Asigments/DAL.DataAccessLayer/Models/Scaffolded/Inventory.cs

public partial class Inventory
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? ZoneId { get; set; }

    public decimal QtyOnHand { get; set; }

    public decimal QtyReserved { get; set; }

    public decimal? QtyAvailable { get; set; }

    public decimal QtyIncoming { get; set; }

    public decimal AvgCost { get; set; }

    public decimal? TotalValue { get; set; }

    public DateTimeOffset LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WarehouseZone? Zone { get; set; }
}
