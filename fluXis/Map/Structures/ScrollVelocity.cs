using System.Collections.Generic;
using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures;

public class ScrollVelocity : ITimedObject, IHasLaneMask
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("multiplier")]
    public double Multiplier { get; set; } = 1;

    [JsonProperty("mask")]
    public List<bool> LaneMask { get; set; } = new();
}
