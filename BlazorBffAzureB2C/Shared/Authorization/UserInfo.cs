﻿using System.Collections.Generic;

namespace BlazorHosted.Shared.Authorization;

public class UserInfo
{
    public static readonly UserInfo Anonymous = new();

    public bool IsAuthenticated { get; set; }

    public string? NameClaimType { get; set; }

    public string? RoleClaimType { get; set; }

    public ICollection<ClaimValue> Claims { get; set; } = new List<ClaimValue>();
}