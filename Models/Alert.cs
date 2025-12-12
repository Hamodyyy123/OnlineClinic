using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Alert
{
    public int AlertId { get; set; }

    public int? PatientId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? AlertType { get; set; }

    public string? VitalType { get; set; }

    public decimal? ReadingValue { get; set; }

    public decimal? Threshold { get; set; }

    public string? Status { get; set; }

    public int? AssignedUserId { get; set; }

    public DateTime? AcknowledgedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? AssignedUser { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Patient? Patient { get; set; }
}
