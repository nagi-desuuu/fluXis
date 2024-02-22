using System.Globalization;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map.Events;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Effect.EffectEdit;

public partial class ShakeEditorPanel : Panel
{
    public ShakeEvent Event { get; set; }
    public EditorMapEvents MapEvents { get; init; }
    public EditorClock EditorClock { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private float beatLength => EditorClock.MapInfo.GetTimingPoint(Event.Time).MsPerBeat;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 400;
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(10),
            Direction = FillDirection.Vertical,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = "Shake Editor",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            FontSize = 30
                        },
                        new ClickableSpriteIcon
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Icon = FontAwesome6.Solid.Question,
                            Size = new Vector2(20),
                            Margin = new MarginPadding(5),
                            Action = () => notifications.SendText("Not implemented yet!")
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Time",
                    Text = Event.Time.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Time = result;
                        else
                            textBox.NotifyError();
                    }
                },
                new BeatsTextBox
                {
                    LabelText = "Duration",
                    Text = (Event.Duration / beatLength).ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Duration = result * beatLength;
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Magnitude",
                    Text = Event.Magnitude.ToStringInvariant(),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            Event.Magnitude = result;
                        else
                            textBox.NotifyError();
                    }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Children = new FluXisButton[]
                    {
                        new()
                        {
                            Width = 100,
                            Height = 40,
                            Text = "Delete",
                            Color = FluXisColors.ButtonRed,
                            Action = () =>
                            {
                                MapEvents.Remove(Event);
                                Hide();
                            }
                        },
                        new()
                        {
                            Width = 100,
                            Height = 40,
                            Text = "Close",
                            Action = Hide
                        }
                    }
                }
            }
        };
    }
}

