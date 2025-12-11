using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("backup_records")]
public partial class BackupRecord
{
    [Key]
    [Column("backup_id")]
    public int BackupId { get; set; }

    [Column("backup_date", TypeName = "datetime")]
    public DateTime? BackupDate { get; set; }

    [Column("backup_file_path")]
    [StringLength(255)]
    public string? BackupFilePath { get; set; }

    [Column("restored")]
    public bool? Restored { get; set; }

    [Column("restored_at", TypeName = "datetime")]
    public DateTime? RestoredAt { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("BackupRecords")]
    public virtual User? CreatedByNavigation { get; set; }
}
