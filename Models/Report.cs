using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? PatientId { get; set; }

    public int? GeneratedBy { get; set; }

    public string? ReportType { get; set; }

    public DateOnly? DateRangeStart { get; set; }

    public DateOnly? DateRangeEnd { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? SignedBy { get; set; }

    public bool? Printable { get; set; }

    public virtual User? GeneratedByNavigation { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual User? SignedByNavigation { get; set; }
}
