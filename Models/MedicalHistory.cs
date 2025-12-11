using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("medical_history")]
public partial class MedicalHistory
{
    [Key]
    [Column("history_id")]
    public int HistoryId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("diagnosis")]
    [StringLength(100)]
    public string? Diagnosis { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("start_date")]
    public DateOnly? StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("MedicalHistories")]
    public virtual Patient? Patient { get; set; }
}
