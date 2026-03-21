using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class LkpDeliveryStatus
{
    public byte StatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();
}
