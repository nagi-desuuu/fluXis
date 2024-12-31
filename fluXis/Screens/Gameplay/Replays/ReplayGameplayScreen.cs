using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Configuration.Experiments;
using fluXis.Database.Maps;
using fluXis.Input;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Online.API.Models.Users;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Input;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Replays;

public partial class ReplayGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    public override bool AllowReverting => true;
    public override bool SubmitScore => false;
    protected override bool UseGlobalOffset => !Config.Get<bool>(FluXisSetting.DisableOffsetInReplay);
    public override APIUser CurrentPlayer => replay.GetPlayer(Users);

    [Resolved]
    private ExperimentConfigManager experiments { get; set; }

    private Replay replay { get; }
    private List<ReplayFrame> frames { get; }
    private Stack<ReplayFrame> handledFrames { get; }
    private List<FluXisGameplayKeybind> currentPressed = new();

    private Bindable<bool> allowSeeking;

    public ReplayGameplayScreen(RealmMap realmMap, List<IMod> mods, Replay replay)
        : base(realmMap, mods)
    {
        this.replay = replay;
        frames = replay.Frames;
        handledFrames = new Stack<ReplayFrame>();
    }

    protected override GameplayInput GetInput() => new ReplayInput(this, RealmMap.KeyCount, Map.IsDual);
    protected override Drawable CreateTextOverlay() => new ReplayOverlay(replay);
    protected override UserActivity GetPlayingActivity() => new UserActivity.WatchingReplay(this, RealmMap, replay.GetPlayer(Users));

    protected override void UpdatePausedState()
    {
        base.UpdatePausedState();

        // set this back to true
        AllowOverlays.Value = true;
    }

    protected override void Update()
    {
        base.Update();

        if (frames.Count == 0)
            return;

        while (frames.Count > 0 && frames[0].Time <= GameplayClock.CurrentTime)
        {
            var frame = frames[0];
            frames.RemoveAt(0);
            handledFrames.Push(frame);
            handlePresses(frame.Actions);
        }

        while (AllowReverting && handledFrames.Count > 0)
        {
            var result = handledFrames.Peek();

            if (GameplayClock.CurrentTime >= result.Time)
                break;

            revertFrame(handledFrames.Pop());
        }
    }

    private void revertFrame(ReplayFrame frame)
    {
        foreach (var keybind in currentPressed)
            Input.ReleaseKey(keybind);

        currentPressed.Clear();
        frames.Insert(0, frame);
    }

    private void handlePresses(List<int> frameActionsInt)
    {
        var frameActions = frameActionsInt.Select(i => (FluXisGameplayKeybind)i).ToList();

        foreach (var keybind in frameActions)
        {
            if (currentPressed.Contains(keybind))
                continue;

            Input.PressKey(keybind);
        }

        foreach (var keybind in currentPressed)
        {
            if (frameActions.Contains(keybind))
                continue;

            Input.ReleaseKey(keybind);
        }

        currentPressed = frameActions;
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        allowSeeking ??= experiments.GetBindable<bool>(ExperimentConfig.Seeking);

        if (!allowSeeking.Value)
            return base.OnPressed(e);

        switch (e.Action)
        {
            case FluXisGlobalKeybind.ReplayPause:
                if (GameplayClock.IsRunning)
                    GameplayClock.Stop();
                else
                    GameplayClock.Start();

                return true;

            case FluXisGlobalKeybind.SeekBackward:
                GameplayClock.Seek(GameplayClock.CurrentTime - 2000);
                return true;

            case FluXisGlobalKeybind.SeekForward:
                GameplayClock.Seek(GameplayClock.CurrentTime + 2000);
                return true;
        }

        return base.OnPressed(e);
    }
}
