using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTag : Container
{
    public virtual Colour4 TagColour => Colour4.White;

    [Resolved]
    private EditorPlayfield playfield { get; set; }

    private EditorTagContainer parent { get; }

    public TimedObject TimedObject { get; }
    protected FluXisSpriteText Text { get; private set; }

    public EditorTag(EditorTagContainer parent, TimedObject timedObject)
    {
        this.parent = parent;
        TimedObject = timedObject;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 86;
        Height = 20;
        Anchor = Anchor.TopRight;
        Origin = Anchor.CentreRight;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Width = 80,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 5,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = TagColour
                    },
                    Text = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = FluXisColors.IsBright(TagColour) ? Colour4.Black : Colour4.White,
                        Alpha = .75f,
                        FontSize = 14
                    }
                }
            },
            new Container
            {
                Size = new Vector2(12),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.TopRight,
                Rotation = 45,
                CornerRadius = 2,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = TagColour
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        Y = parent.ToLocalSpace(playfield.HitObjectContainer.ScreenSpacePositionAtTime(TimedObject.Time, 0)).Y;
    }
}