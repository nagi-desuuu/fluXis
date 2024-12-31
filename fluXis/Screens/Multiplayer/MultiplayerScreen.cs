using fluXis.Audio;
using fluXis.Graphics.Background;
using fluXis.Map;
using fluXis.Screens.Multiplayer.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer;

[Cached]
public partial class MultiplayerScreen : FluXisScreen
{
    public override float Zoom => 1.1f;
    public override float ParallaxStrength => .05f;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .2f;
    public override bool AllowMusicControl => false;
    public override bool AllowMusicPausing => screenStack?.CurrentScreen is MultiSubScreen { AllowMusicPausing: true };
    public override bool AllowExit => canExit();

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Cached]
    private MultiplayerMenuMusic menuMusic = new();

    public long TargetLobby { get; init; }
    public string LobbyPassword { get; init; }

    private ScreenStack screenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            menuMusic,
            screenStack = new ScreenStack
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var modes = new MultiModeSelect();
        screenStack.Push(modes);

        if (TargetLobby > 0)
            modes.OpenList(TargetLobby, LobbyPassword);

        screenStack.ScreenExited += (_, _) =>
        {
            if (screenStack.CurrentScreen == null)
                this.Exit();
        };
    }

    private bool canExit()
    {
        while (screenStack.CurrentScreen != null && screenStack.CurrentScreen is not MultiModeSelect)
        {
            var subScreen = (MultiSubScreen)screenStack.CurrentScreen;
            if (subScreen.IsLoaded && subScreen.OnExiting(null))
                return false;

            subScreen.Exit();
        }

        return true;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        using (BeginDelayedSequence(ENTER_DELAY))
            screenStack.FadeIn(FADE_DURATION);

        globalClock.VolumeOut(400).OnComplete(c => c.Stop());
        backgrounds.AddBackgroundFromMap(null);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnResuming(null);
        this.FadeIn();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnSuspending(null);
        this.Delay(800).FadeOut();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (!canExit()) return true;

        menuMusic.StopAll();
        this.Delay(FADE_DURATION).FadeOut();

        globalClock.Start();
        globalClock.VolumeIn(FADE_DURATION);
        backgrounds.AddBackgroundFromMap(mapStore.CurrentMapSet?.Maps[0]);
        return false;
    }
}
