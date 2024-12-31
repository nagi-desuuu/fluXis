using fluXis.Online.API.Models.Other;
using fluXis.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Other;

public class AchievementPacket : IPacket
{
    public string ID => PacketIDs.ACHIEVEMENT;

    [JsonProperty("achievement")]
    public Achievement Achievement { get; set; } = null!;

    public static AchievementPacket Create(Achievement achievement)
        => new() { Achievement = achievement };
}
