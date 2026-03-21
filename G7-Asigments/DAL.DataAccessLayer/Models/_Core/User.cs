using DAL.DataAccessLayer.Models._Lookup;

namespace DAL.DataAccessLayer.Models._Core;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public byte RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public virtual LkpUserRole Role { get; set; } = null!;
}
