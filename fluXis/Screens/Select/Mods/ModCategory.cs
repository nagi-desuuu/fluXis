using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Mods;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Mods;

public partial class ModCategory : Container
{
    public ModSelector Selector { get; init; }

    public LocalisableString Label { get; init; }
    public string HexColour { get; init; }
    public IMod[] Mods { get; init; }

    private FillFlowContainer mods;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding { Horizontal = 5 };

        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Colour4.FromHex(HexColour),
                    RelativeSizeAxes = Axes.Both
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 30,
                    Padding = new MarginPadding { Vertical = 2, Horizontal = 10 },
                    Child = new FluXisSpriteText
                    {
                        Text = Label,
                        Colour = FluXisColors.TextDark,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        FontSize = 22
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 2, Bottom = 2, Top = 30 },
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        CornerRadius = 8,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                Colour = FluXisColors.Background2,
                                RelativeSizeAxes = Axes.Both
                            },
                            mods = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Padding = new MarginPadding(5),
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 5)
                            }
                        }
                    }
                }
            }
        };

        foreach (var mod in Mods)
        {
            var modEntry = new ModEntry
            {
                Mod = mod,
                HexColour = HexColour,
                Selector = Selector
            };

            mods.Add(modEntry);
            Selector.AddMapping(mod, modEntry);
        }
    }
}
