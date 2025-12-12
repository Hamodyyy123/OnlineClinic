using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class AuditLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? ResourceType { get; set; }

    public int? ResourceId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Details { get; set; }

    public string? IpAddress { get; set; }

    public virtual User? User { get; set; }
}
