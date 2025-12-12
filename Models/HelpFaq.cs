using System;
using System.Collections.Generic;

namespace OnlineClinic.Models;

public partial class HelpFaq
{
    public int FaqId { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public DateTime? CreatedAt { get; set; }
}
