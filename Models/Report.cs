using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("reports")]
public partial class Report
{
    [Key]
    [Column("report_id")]
    public int ReportId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("generated_by")]
    public int? GeneratedBy { get; set; }

    [Column("report_type")]
    [StringLength(50)]
    public string? ReportType { get; set; }

    [Column("date_range_start")]
    public DateOnly? DateRangeStart { get; set; }

    [Column("date_range_end")]
    public DateOnly? DateRangeEnd { get; set; }

    [Column("file_path")]
    [StringLength(255)]
    public string? FilePath { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("signed_by")]
    public int? SignedBy { get; set; }

    [Column("printable")]
    public bool? Printable { get; set; }

    [ForeignKey("GeneratedBy")]
    [InverseProperty("ReportGeneratedByNavigations")]
    public virtual User? GeneratedByNavigation { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("Reports")]
    public virtual Patient? Patient { get; set; }

    [ForeignKey("SignedBy")]
    [InverseProperty("ReportSignedByNavigations")]
    public virtual User? SignedByNavigation { get; set; }
}
