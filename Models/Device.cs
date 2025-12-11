using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("devices")]
public partial class Device
{
    [Key]
    [Column("device_id")]
    public int DeviceId { get; set; }

    [Column("device_type")]
    [StringLength(50)]
    public string? DeviceType { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("last_seen", TypeName = "datetime")]
    public DateTime? LastSeen { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("Devices")]
    public virtual Patient? Patient { get; set; }

    [InverseProperty("Device")]
    public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
}
