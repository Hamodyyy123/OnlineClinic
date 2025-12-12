using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class RolePermission
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public int? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }

    public virtual UserRole? Role { get; set; }
}
