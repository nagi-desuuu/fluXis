﻿using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Clubs;

public class APIClubStatistics
{
    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("score")]
    public double TotalScore { get; set; }

    [JsonProperty("rank")]
    public double Rank { get; set; }

    [JsonProperty("claims")]
    public long TotalClaims { get; set; }

    [JsonProperty("claim-percent")]
    public double ClaimPercentage { get; set; }
}
