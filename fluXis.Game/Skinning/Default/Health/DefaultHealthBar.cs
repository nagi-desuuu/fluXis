using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Scoring.Processing.Health;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game.Skinning.Default.Health;

public partial class DefaultHealthBar : Container
{
    private HealthProcessor processor { get; }

    private FluXisSpriteText text;

    private readonly ColourInfo drainGradient = ColourInfo.GradientHorizontal(Colour4.FromHex("#40aef8"), Colour4.FromHex("#751010"));
    private double drainRate;

    public DefaultHealthBar(HealthProcessor processor)
    {
        this.processor = processor;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        CornerRadius = 10;
        Masking = true;
        BorderColour = ColourInfo.GradientVertical(FluXisColors.Accent, FluXisColors.Accent4);
        BorderThickness = 4;
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Colour = FluXisColors.Accent,
                FontSize = 18,
                Y = 10
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        text.Text = $"{(int)Math.Round(processor.SmoothHealth)}";

        switch (processor)
        {
            case RequirementHeathProcessor requirement:
                BorderColour = text.Colour = Colour4.FromHex(requirement.RequirementReached ? "#40aef8" : "#40f840");
                break;

            case DrainHealthProcessor drain:
                //smoothen the drain rate to avoid flickering
                drainRate = Interpolation.Lerp(drainRate, drain.HealthDrainRate, Math.Exp(-0.001f * Clock.ElapsedFrameTime));
                BorderColour = text.Colour = drainGradient.Interpolate(new Vector2((Math.Clamp((float)drainRate, -1, 1) + 1) / 2f, 0));
                break;
        }
    }
}
