using fluXis.Overlay.Settings;
using osu.Framework.Allocation;

namespace fluXis.Tests.Settings;

public partial class TestSettingsMenu : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var menu = new SettingsMenu();
        Add(menu);

        AddStep("Show", menu.Show);
        AddStep("Hide", menu.Hide);
    }
}
