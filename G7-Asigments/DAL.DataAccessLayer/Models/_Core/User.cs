using DAL.DataAccessLayer.Models._Lookup;

namespace DAL.DataAccessLayer.Models._Core;

public class User
{
    public Guid Id { get; set; }
    public string? StaffCode { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = null!;
    public byte RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual LkpUserRole Role { get; set; } = null!;
}
