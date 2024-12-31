﻿using System;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Auth;

public class RegisterResponse
{
    [JsonProperty("token")]
    public string AccessToken { get; init; } = null!;

    public RegisterResponse(string token)
    {
        AccessToken = token;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public RegisterResponse()
    {
    }
}
