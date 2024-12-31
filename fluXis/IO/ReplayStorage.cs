using System;
using System.IO;
using fluXis.Replays;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.IO;

public class ReplayStorage
{
    private Storage storage { get; }

    public ReplayStorage(Storage storage)
    {
        this.storage = storage;
    }

    public bool Exists(Guid id)
    {
        var path = getPath(id);
        return storage.Exists(path);
    }

    [CanBeNull]
    public Replay Get(Guid scoreId)
    {
        var path = getPath(scoreId);

        if (!Exists(scoreId))
            return null;

        var json = File.ReadAllText(storage.GetFullPath(path));
        return json.Deserialize<Replay>();
    }

    public void Save(Replay replay, Guid scoreId)
    {
        var path = getPath(scoreId);
        var json = replay.Serialize();
        File.WriteAllText(storage.GetFullPath(path), json);
    }

    private string getPath(Guid scoreId) => $"{scoreId}.frp";
}
