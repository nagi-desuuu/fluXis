using System;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel.Types;

public partial class ButtonPanel : Panel, ICloseable
{
    public IconUsage Icon { get; init; } = FontAwesome.Solid.QuestionCircle;
    public LocalisableString Text { get; init; }
    public LocalisableString SubText { get; init; }
    public ButtonData[] Buttons { get; init; } = Array.Empty<ButtonData>();

    public Action<FluXisTextFlow> CreateSubText { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 490;
        AutoSizeAxes = Axes.Y;

        FluXisTextFlow subTextFlow;

        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;
        Content.Padding = new MarginPadding(0);
        Content.Children = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(20) { Top = 30 },
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Icon = Icon,
                        Size = new Vector2(64),
                        Margin = new MarginPadding { Bottom = 10 }
                    },
                    new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        Text = Text,
                        FontSize = FluXisSpriteText.GetWebFontSize(20),
                        Shadow = false
                    },
                    subTextFlow = new FluXisTextFlow
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        TextAnchor = Anchor.TopCentre,
                        FontSize = FluXisSpriteText.GetWebFontSize(14),
                        Alpha = string.IsNullOrEmpty(SubText.ToString()) && CreateSubText == null ? 0 : .8f,
                        Shadow = false
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(10),
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding { Top = 20 },
                        ChildrenEnumerable = Buttons.Select(b => new FluXisButton
                        {
                            Width = 450,
                            Height = 50,
                            Data = b,
                            FontSize = FluXisSpriteText.GetWebFontSize(16),
                            Action = () =>
                            {
                                b.Action?.Invoke();
                                if (!Loading) Hide();
                            }
                        })
                    }
                }
            }
        };

        CreateSubText ??= f => f.Text = SubText;
        CreateSubText.Invoke(subTextFlow);
    }

    protected override bool OnClick(ClickEvent e) => true;

    public void Close()
    {
        if (Loading)
            return;

        var last = Buttons.Last();
        last.Action?.Invoke();
        Hide();
    }
}
