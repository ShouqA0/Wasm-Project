using System;
using System.Collections.Generic;

namespace WasmProject.Models;

public partial class TrustedStore
{
    public int Id { get; set; }

    public string SenderId { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Category { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
