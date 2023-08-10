using fluXis.Game.Graphics;
using fluXis.Game.Screens.Edit.Tabs.Charting;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class ChartingTab : EditorTab
{
    private LoadingIcon loadingIcon;

    public ChartingTab(Editor screen)
        : base(screen)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = loadingIcon = new LoadingIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(50)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(new ChartingContainer(), container =>
        {
            loadingIcon.FadeOut(200);

            AddInternal(container);
            container.FadeInFromZero(200);
        });
    }
}