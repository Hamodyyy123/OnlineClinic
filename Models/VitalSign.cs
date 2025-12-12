using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class VitalSign
{
    public int VitalId { get; set; }

    public int? PatientId { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? HeartRate { get; set; }

    public int? BloodPressureSystolic { get; set; }

    public int? BloodPressureDiastolic { get; set; }

    public decimal? Temperature { get; set; }

    public int? OxygenSaturation { get; set; }

    public string? Source { get; set; }

    public int? DeviceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Device? Device { get; set; }

    public virtual Patient? Patient { get; set; }
}
