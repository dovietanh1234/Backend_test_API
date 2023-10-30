using System;
using System.Collections.Generic;

namespace WEB_API.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public bool? IsUsed { get; set; }

    public bool? IsRevoked { get; set; }

    public DateTime? IsUsedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public virtual User2 User { get; set; } = null!;
}
