using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int? UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public bool? Active { get; set; }

    public string? Token { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
