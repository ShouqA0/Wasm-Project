using System;
using System.Collections.Generic;

namespace WasmProject.Models;

public partial class LostItem
{
    public int LostItemId { get; set; }

    public int? PropertyId { get; set; }

    public DateTime? LostDate { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Notes { get; set; }

    public virtual Property? Property { get; set; }
}
