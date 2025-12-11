using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("notifications")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("alert_id")]
    public int? AlertId { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("delivery_method")]
    [StringLength(20)]
    public string? DeliveryMethod { get; set; }

    [Column("delivered_at", TypeName = "datetime")]
    public DateTime? DeliveredAt { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("AlertId")]
    [InverseProperty("Notifications")]
    public virtual Alert? Alert { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User? User { get; set; }
}
