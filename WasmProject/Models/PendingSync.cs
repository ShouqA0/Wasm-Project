using System;
using System.Collections.Generic;
namespace WasmProject.Models;

public partial class PendingSync
{
    public int Id { get; set; }

    public string UserPhone { get; set; } = null!;

    public string DetectedItemName { get; set; } = null!;

    public string SenderId { get; set; } = null!;

    public string SourceType { get; set; } = null!;

    public string? Status { get; set; }

    public string? RawContent { get; set; }

    public DateTime? CreatedAt { get; set; }
}
