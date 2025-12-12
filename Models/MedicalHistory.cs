using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class MedicalHistory
{
    public int HistoryId { get; set; }

    public int? PatientId { get; set; }

    public string? Diagnosis { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient? Patient { get; set; }
}
