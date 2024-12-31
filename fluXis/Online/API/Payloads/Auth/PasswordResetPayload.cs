﻿using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Auth;

#nullable enable

public class PasswordResetPayload
{
    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }
}
