using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Online.Multiplayer;

public abstract partial class MultiplayerClient : Component
{
    public event Action<MultiplayerParticipant> OnUserJoin;
    public event Action<MultiplayerParticipant> OnUserLeave;
    public event Action<long, MultiplayerUserState> OnUserStateChange;

    // public event Action RoomUpdated;

    public event Action<APIMap> OnMapChange;
    public event Action<string> MapChangeFailed;

    public event Action OnStart;
    public event Action<long, int> OnScore;
    public event Action<List<ScoreInfo>> OnResultsReady;

    public event Action OnDisconnect;

    public virtual APIUser Player => APIUser.Dummy;
    public MultiplayerRoom Room { get; set; }

    public async Task Create(string name, long mapid, string hash)
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot create a room while already in one");

        Room = await CreateRoom(name, mapid, hash);
    }

    public async Task Join(long id, string password = "")
    {
        if (Room != null)
            throw new InvalidOperationException("Cannot join a room while already in one");

        Room = await JoinRoom(id, password);
    }

    protected Task UserJoined(MultiplayerParticipant participant)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (Room.Participants.Any(u => u.ID == participant.ID))
                return;

            Room.Participants.Add(participant);

            OnUserJoin?.Invoke(participant);
        });

        return Task.CompletedTask;
    }

    protected Task UserLeft(long id) => handleLeave(id, OnUserLeave);

    protected Task UserStateChanged(long id, MultiplayerUserState state)
    {
        Schedule(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            var user = participant as MultiplayerParticipant;
            user!.State = state;

            OnUserStateChange?.Invoke(id, state);
        });

        return Task.CompletedTask;
    }

    protected Task SettingsChanged(MultiplayerRoom room) => Task.CompletedTask;

    protected Task MapChanged(bool success, APIMap map, string error)
    {
        Schedule(() =>
        {
            if (Room == null)
                return;

            if (!success)
            {
                MapChangeFailed?.Invoke(error);
                OnMapChange?.Invoke(Room.Map);
                return;
            }

            Room.Map = map;
            OnMapChange?.Invoke(map);
        });

        return Task.CompletedTask;
    }

    protected Task Starting()
    {
        Schedule(() =>
        {
            Room.Scores = new List<ScoreInfo>();
            Room.Scores.AddRange(Room.Participants.Where(p => p.ID != Player.ID).Select(p => new ScoreInfo { PlayerID = p.ID }));

            OnStart?.Invoke();
        });

        return Task.CompletedTask;
    }

    protected Task ScoreUpdated(long id, int score)
    {
        Schedule(() =>
        {
            if (id == Player.ID)
                return;

            OnScore?.Invoke(id, score);
        });

        return Task.CompletedTask;
    }

    private Task handleLeave(long id, Action<MultiplayerParticipant> callback)
    {
        Scheduler.Add(() =>
        {
            if (Room?.Participants.FirstOrDefault(u => u.ID == id) is not { } participant)
                return;

            Room.Participants.Remove(participant);

            callback?.Invoke(participant as MultiplayerParticipant);
        }, false);

        return Task.CompletedTask;
    }

    protected Task ResultsReady(List<ScoreInfo> scores)
    {
        Logger.Log($"Received results for {scores.Count} players", LoggingTarget.Network, LogLevel.Debug);
        Schedule(() => OnResultsReady?.Invoke(scores));
        return Task.CompletedTask;
    }

    protected void Disconnect()
    {
        Room = null;
        Scheduler.ScheduleIfNeeded(() => OnDisconnect?.Invoke());
    }

    #region Abstract Methods

    protected abstract Task<MultiplayerRoom> JoinRoom(long id, string password);
    protected abstract Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash);
    public abstract Task LeaveRoom();
    public abstract Task ChangeMap(long map, string hash);
    public abstract Task UpdateScore(int score);
    public abstract Task Finish(ScoreInfo score);

    #endregion
}
