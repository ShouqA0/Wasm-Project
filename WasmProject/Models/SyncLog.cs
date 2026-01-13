using System;
using System.Collections.Generic;

namespace WasmProject.Models;

public partial class SyncLog
{
    public int Id { get; set; }

    public DateTime? LogTimestamp { get; set; }

    public string? ActionType { get; set; }

    public string? Status { get; set; }

    public string? Details { get; set; }
}
