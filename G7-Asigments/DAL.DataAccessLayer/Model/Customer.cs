using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Model;

public partial class Customer
{
    public Guid Id { get; set; }

    public string CustomerCode { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? TaxCode { get; set; }

    public string? ContactPerson { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? CustomerType { get; set; }

    public decimal? CreditLimit { get; set; }

    public string? PaymentTerms { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();

    public virtual ICollection<GoodsIssue> GoodsIssues { get; set; } = new List<GoodsIssue>();

    public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
