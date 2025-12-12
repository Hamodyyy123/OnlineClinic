using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? AlertId { get; set; }

    public string? Message { get; set; }

    public string? DeliveryMethod { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Alert? Alert { get; set; }

    public virtual User? User { get; set; }
}
