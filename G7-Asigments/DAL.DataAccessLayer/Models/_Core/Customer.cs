namespace DAL.DataAccessLayer.Models._Core;

public class Customer
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
    public string? Country { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
