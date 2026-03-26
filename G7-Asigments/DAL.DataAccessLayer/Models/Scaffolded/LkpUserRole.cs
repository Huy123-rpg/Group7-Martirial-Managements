using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class LkpUserRole
{
    public byte RoleId { get; set; }

    public string RoleCode { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
