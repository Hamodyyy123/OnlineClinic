using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("medical_notes")]
public partial class MedicalNote
{
    [Key]
    [Column("note_id")]
    public int NoteId { get; set; }

    [Column("patient_id")]
    public int? PatientId { get; set; }

    [Column("author_id")]
    public int? AuthorId { get; set; }

    [Column("timestamp", TypeName = "datetime")]
    public DateTime? Timestamp { get; set; }

    [Column("note_type")]
    [StringLength(50)]
    public string? NoteType { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("MedicalNotes")]
    public virtual User? Author { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("MedicalNotes")]
    public virtual Patient? Patient { get; set; }
}
