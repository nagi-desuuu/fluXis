﻿using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Account;

#nullable enable

public class RegisterPacket : IPacket
{
    public string ID => PacketIDs.REGISTER;

    #region Client2Server

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }

    #endregion

    #region Server2Client

    [JsonProperty("token")]
    public string Token { get; set; } = null!;

    [JsonProperty("user")]
    public APIUser User { get; set; } = null!;

    #endregion

    public static RegisterPacket CreateC2S(string username, string email, string password)
        => new() { Username = username, Email = email, Password = password };

    public static RegisterPacket CreateS2C(string token, APIUser user) => new() { Token = token, User = user };
}
