using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox.Hitsound;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class Toolbox : ExpandingContainer
{
    private const int padding = 5;
    private const int size_closed = 64 + padding * 4;
    private const int size_open = 300 + padding * 4;

    protected override double HoverDelay => 500;

    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    private FillFlowContainer<ToolboxCategory> categories;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowMediumNoOffset;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(padding),
                ScrollbarVisible = false,
                Child = categories = new FillFlowContainer<ToolboxCategory>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10),
                    Children = new ToolboxCategory[]
                    {
                        new()
                        {
                            Title = "Tools",
                            Icon = FontAwesome6.Solid.Pen,
                            Tools = chartingContainer.Tools
                        },
                        new()
                        {
                            Title = "Effects",
                            Icon = FontAwesome6.Solid.WandMagicSparkles,
                            Tools = chartingContainer.EffectTools
                        },
                        new ToolboxHitsoundCategory(),
                        new ToolboxSnapCategory()
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : size_closed, 500, Easing.OutQuart);
            categories.ForEach(d => d.OnSizeChanged(v.NewValue));
        }, true);
    }
}
