using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics;

public partial class FluXisButton : ClickableContainer
{
    public int FontSize { get; set; } = 24;
    public string Text { get; set; } = "Default Text";
    public Colour4 Color { get; set; } = FluXisColors.Surface2;

    public ButtonData Data
    {
        set
        {
            Text = value.Text;
            Action = value.Action;
            Color = value.Color;
        }
    }

    private Box hoverBox;
    private CircularContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color
                },
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new FluXisSpriteText
                    {
                        Text = Text,
                        FontSize = FontSize,
                        Shadow = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(0.4f)
                .FadeTo(.2f, 400);

        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(0.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(0.95f, 4000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 800, Easing.OutElastic);
    }
}

public class ButtonData
{
    public string Text { get; init; } = "Default Text";
    public Colour4 Color { get; init; } = FluXisColors.Surface2;
    public Action Action { get; init; } = () => { };
}
