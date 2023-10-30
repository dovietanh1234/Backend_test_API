using System;
using System.Collections.Generic;

namespace WEB_API.Entities;

public partial class User2
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telephone { get; set; }

    public string? Address { get; set; }

    public string? Cỉty { get; set; }

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
