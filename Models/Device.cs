using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Device
{
    public int DeviceId { get; set; }

    public string? DeviceType { get; set; }

    public string? Status { get; set; }

    public DateTime? LastSeen { get; set; }

    public int? PatientId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
}
