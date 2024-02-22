using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Edit;

public partial class TestEditor : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    private ScreenStack screenStack { get; } = new() { RelativeSizeAxes = Axes.Both };
    private EditorLoader editor { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);
        Add(backgrounds);

        Add(screenStack);

        var map = maps.GetFromGuid("4820fd48-69b3-4c05-983b-46923697680f")?
            .Maps.FirstOrDefault();

        editor = map is not null ? new EditorLoader(map, map.GetMapInfo()) : new EditorLoader();
        screenStack.Push(editor);
    }
}
