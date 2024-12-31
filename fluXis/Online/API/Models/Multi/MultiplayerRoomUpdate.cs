using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Multi;

public class MultiplayerRoomUpdate
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("data")]
    public dynamic Data { get; set; }
}
