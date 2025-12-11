using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineClinic.Models;

[Table("help_faqs")]
public partial class HelpFaq
{
    [Key]
    [Column("faq_id")]
    public int FaqId { get; set; }

    [Column("question")]
    public string? Question { get; set; }

    [Column("answer")]
    public string? Answer { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
}
