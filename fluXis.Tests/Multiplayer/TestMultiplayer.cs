using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Screens.Multiplayer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Tests.Multiplayer;

public partial class TestMultiplayer : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);

        var stack = new ScreenStack { RelativeSizeAxes = Axes.Both };

        Children = new Drawable[]
        {
            backgrounds,
            stack,
            panels
        };

        AddStep("Push MultiplayerScreen", () => stack.Push(new MultiplayerScreen()));
    }
}
