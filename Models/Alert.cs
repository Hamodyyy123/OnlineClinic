using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("alerts")]
public partial class Alert
{
    [Key]
    [Column("alert_id")]
    public int AlertId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("timestamp", TypeName = "datetime")]
    public DateTime? Timestamp { get; set; }

    [Column("alert_type")]
    [StringLength(50)]
    public string? AlertType { get; set; }

    [Column("vital_type")]
    [StringLength(50)]
    public string? VitalType { get; set; }

    [Column("reading_value", TypeName = "decimal(6, 2)")]
    public decimal? ReadingValue { get; set; }

    [Column("threshold", TypeName = "decimal(6, 2)")]
    public decimal? Threshold { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("assigned_user_id")]
    public int? AssignedUserId { get; set; }

    [Column("acknowledged_at", TypeName = "datetime")]
    public DateTime? AcknowledgedAt { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("AssignedUserId")]
    [InverseProperty("Alerts")]
    public virtual User? AssignedUser { get; set; }

    [InverseProperty("Alert")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("PatientId")]
    [InverseProperty("Alerts")]
    public virtual Patient? Patient { get; set; }
}
