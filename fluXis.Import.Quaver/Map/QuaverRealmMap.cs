using System.IO;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMap : RealmMap
{
    public override MapInfo GetMapInfo()
    {
        var path = MapSet.GetPathForFile(FileName);
        string yaml = File.ReadAllText(path);
        return QuaverImport.ParseFromYaml(yaml).ToMapInfo();
    }
}
