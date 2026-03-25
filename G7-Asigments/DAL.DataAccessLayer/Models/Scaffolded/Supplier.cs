using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;G7-Asigments/DAL.DataAccessLayer/Models/Scaffolded/Supplier.cs

public partial class Supplier
{
    public Guid Id { get; set; }

    public string SupplierCode { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public string? TaxCode { get; set; }

    public string? ContactPerson { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public int? LeadTimeDays { get; set; }

    public string? PaymentTerms { get; set; }

    public decimal? Rating { get; set; }

    public bool IsActive { get; set; }

    public bool PortalEnabled { get; set; }

    public Guid? PortalUserId { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<AiReorderSuggestion> AiReorderSuggestions { get; set; } = new List<AiReorderSuggestion>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual User? PortalUser { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
