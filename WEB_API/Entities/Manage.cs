using System;
using System.Collections.Generic;

namespace WEB_API.Entities;

public partial class Manage
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Avatar { get; set; }
}
