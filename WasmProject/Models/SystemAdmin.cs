using System;
using System.Collections.Generic;

namespace WasmProject.Models;

public partial class SystemAdmin
{
    public int AdminId { get; set; }

    public string AdminUser { get; set; } = null!;

    public string AdminPasswordHash { get; set; } = null!;

    public string? AdminName { get; set; }

    public string? Email { get; set; }
}
