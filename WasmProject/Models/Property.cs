using System;
using System.Collections.Generic;

namespace WasmProject.Models;

public partial class Property
{
    public int PropertyId { get; set; }

    public int? UserId { get; set; }

    public string? Category { get; set; }

    public string? Brand { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? SourceType { get; set; }

    public string? VerifiedStoreName { get; set; }

    public string? SourceIdentifier { get; set; }

    public bool? IsVerified { get; set; }

    public string? ExternalReferenceId { get; set; }

    public DateTime? SyncTimestamp { get; set; }

    public virtual ICollection<LostItem> LostItems { get; set; } = new List<LostItem>();

    public virtual UserDb? User { get; set; }
}
