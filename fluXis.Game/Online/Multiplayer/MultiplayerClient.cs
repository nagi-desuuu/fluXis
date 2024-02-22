using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Multiplayer;

public abstract partial class MultiplayerClient : Component, IMultiplayerClient
{
    public event Action<APIUserShort> UserJoined;
    public event Action<APIUserShort> UserLeft;
    public event Action RoomUpdated;
    public event Action<long, bool> ReadyStateChanged;
    public event Action Starting;
    public event Action<List<ScoreInfo>> ResultsReady;

    public virtual APIUserShort Player => APIUserShort.Dummy;
    public MultiplayerRoom Room { get; set; }

    Task IMultiplayerClient.UserJoined(APIUserShort user)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (Room.Users.Any(u => u.ID == user.ID))
                return;

            Room.Users.Add(user);

            UserJoined?.Invoke(user);
            RoomUpdated?.Invoke();
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.UserLeft(long id) => handleLeave(id, UserLeft);

    Task IMultiplayerClient.SettingsChanged(MultiplayerRoom room) => Task.CompletedTask;

    Task IMultiplayerClient.ReadyStateChanged(long userId, bool isReady)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            ReadyStateChanged?.Invoke(userId, isReady);
            RoomUpdated?.Invoke();
        });

        return Task.CompletedTask;
    }

    Task IMultiplayerClient.Starting()
    {
        Schedule(() => Starting?.Invoke());
        return Task.CompletedTask;
    }

    private Task handleLeave(long id, Action<APIUserShort> callback)
    {
        Scheduler.Add(() =>
        {
            var user = Room?.Users.FirstOrDefault(u => u.ID == id);

            if (user == null)
                return;

            Room.Users.Remove(user);

            callback?.Invoke(user);
            RoomUpdated?.Invoke();
        }, false);

        return Task.CompletedTask;
    }

    public virtual Task Finished(ScoreInfo score) => Task.CompletedTask;

    Task IMultiplayerClient.ResultsReady(List<ScoreInfo> scores)
    {
        Logger.Log($"Received results for {scores.Count} players", LoggingTarget.Network, LogLevel.Debug);
        Schedule(() => ResultsReady?.Invoke(scores));
        return Task.CompletedTask;
    }
}
