using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("vital_signs")]
public partial class VitalSign
{
    [Key]
    [Column("vital_id")]
    public int VitalId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("timestamp", TypeName = "datetime")]
    public DateTime? Timestamp { get; set; }

    [Column("heart_rate")]
    public int? HeartRate { get; set; }

    [Column("blood_pressure_systolic")]
    public int? BloodPressureSystolic { get; set; }

    [Column("blood_pressure_diastolic")]
    public int? BloodPressureDiastolic { get; set; }

    [Column("temperature", TypeName = "decimal(4, 2)")]
    public decimal? Temperature { get; set; }

    [Column("oxygen_saturation")]
    public int? OxygenSaturation { get; set; }

    [Column("source")]
    [StringLength(20)]
    public string? Source { get; set; }

    [Column("device_id")]
    public int? DeviceId { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("DeviceId")]
    [InverseProperty("VitalSigns")]
    public virtual Device? Device { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("VitalSigns")]
    public virtual Patient? Patient { get; set; }
}
