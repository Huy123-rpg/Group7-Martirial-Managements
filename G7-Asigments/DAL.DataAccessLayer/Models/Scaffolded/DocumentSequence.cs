using System;
using System.Collections.Generic;

namespace DAL.DataAccessLayer.Models;

public partial class DocumentSequence
{
    public string Prefix { get; set; } = null!;

    public int CurrentNumber { get; set; }

    public int SeqYear { get; set; }

    public string FormatTemplate { get; set; } = null!;
}
