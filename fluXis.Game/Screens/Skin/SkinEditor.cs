using System.Globalization;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens.Edit.MenuBar;
using fluXis.Game.Screens.Skin.UI;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Skin;

public partial class SkinEditor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => 1f;
    public override float ParallaxStrength => .05f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .3f;
    public override bool AllowMusicControl => true;

    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private Skinning.Json.SkinJson skinJson { get; set; }
    private Bindable<int> keyMode { get; } = new(4);

    private EditorMenuBar menuBar;
    private PopoverContainer content;

    private SkinEditorPlayfield playfield;

    private SkinEditorTextBox hitPositionTextBox;
    private SkinEditorTextBox columnWidthTextBox;

    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        skinJson = skinManager.SkinJson.Copy();

        keyMode.BindValueChanged(e =>
        {
            playfield.KeyMode = e.NewValue;
            playfield.Reload();

            hitPositionTextBox.TextBox.Text = skinJson.GetKeymode(keyMode.Value).HitPosition.ToString();
            columnWidthTextBox.TextBox.Text = skinJson.GetKeymode(keyMode.Value).ColumnWidth.ToString();
        });

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10) { Top = 55 },
                Child = content = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new Dimension[]
                        {
                            new(),
                            new(GridSizeMode.Absolute, 20),
                            new(GridSizeMode.Absolute, 320)
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                playfield = new SkinEditorPlayfield
                                {
                                    SkinJson = skinJson,
                                    SkinManager = skinManager,
                                    KeyMode = keyMode.Value
                                },
                                Empty(),
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    CornerRadius = 10,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = FluXisColors.Background2
                                        },
                                        new FluXisScrollContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Padding = new MarginPadding(10),
                                            Child = new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Spacing = new Vector2(0, 10),
                                                Children = new Drawable[]
                                                {
                                                    new FluXisSpriteText
                                                    {
                                                        Text = "Keymode-specific",
                                                        FontSize = 34,
                                                        Margin = new MarginPadding { Top = 10 }
                                                    },
                                                    hitPositionTextBox = new SkinEditorTextBox
                                                    {
                                                        Text = "Hit Position",
                                                        DefaultText = skinJson.GetKeymode(keyMode.Value).HitPosition.ToString(),
                                                        OnTextChanged = () =>
                                                        {
                                                            if (int.TryParse(hitPositionTextBox.TextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int y))
                                                                skinJson.GetKeymode(keyMode.Value).HitPosition = y;
                                                            else
                                                                hitPositionTextBox.TextBox.NotifyError();
                                                        }
                                                    },
                                                    columnWidthTextBox = new SkinEditorTextBox
                                                    {
                                                        Text = "Column Width",
                                                        DefaultText = skinJson.GetKeymode(keyMode.Value).ColumnWidth.ToString(),
                                                        OnTextChanged = () =>
                                                        {
                                                            if (int.TryParse(columnWidthTextBox.TextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int w))
                                                                skinJson.GetKeymode(keyMode.Value).ColumnWidth = w;
                                                            else
                                                                columnWidthTextBox.TextBox.NotifyError();
                                                        }
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Text = "Judgements",
                                                        FontSize = 34,
                                                        Margin = new MarginPadding { Top = 10 }
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Flawless),
                                                        Text = "Flawless",
                                                        OnColorChanged = c => skinJson.Judgements.Flawless = c.ToHex()
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Perfect),
                                                        Text = "Perfect",
                                                        OnColorChanged = c => skinJson.Judgements.Perfect = c.ToHex()
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Great),
                                                        Text = "Great",
                                                        OnColorChanged = c => skinJson.Judgements.Great = c.ToHex()
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Alright),
                                                        Text = "Alright",
                                                        OnColorChanged = c => skinJson.Judgements.Alright = c.ToHex()
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Okay),
                                                        Text = "Okay",
                                                        OnColorChanged = c => skinJson.Judgements.Okay = c.ToHex()
                                                    },
                                                    new SkinEditorColor
                                                    {
                                                        Color = skinJson.GetColorForJudgement(Judgement.Miss),
                                                        Text = "Miss",
                                                        OnColorChanged = c => skinJson.Judgements.Miss = c.ToHex()
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            menuBar = new EditorMenuBar
            {
                Items = new[]
                {
                    new MenuItem("File")
                    {
                        Items = new[]
                        {
                            new MenuItem("Save", save),
                            new MenuItem("Exit", this.Exit)
                        }
                    },
                    new MenuItem("Key Mode")
                    {
                        Items = new[]
                        {
                            new MenuItem("1 Key", () => keyMode.Value = 1),
                            new MenuItem("2 Keys", () => keyMode.Value = 2),
                            new MenuItem("3 Keys", () => keyMode.Value = 3),
                            new MenuItem("4 Keys", () => keyMode.Value = 4),
                            new MenuItem("5 Keys", () => keyMode.Value = 5),
                            new MenuItem("6 Keys", () => keyMode.Value = 6),
                            new MenuItem("7 Keys", () => keyMode.Value = 7),
                            new MenuItem("8 Keys", () => keyMode.Value = 8),
                            new MenuItem("9 Keys", () => keyMode.Value = 9),
                            new MenuItem("10 Keys", () => keyMode.Value = 10)
                        }
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        skinManager.CanChangeSkin = false;

        this.FadeInFromZero(200);
        menuBar.MoveToY(-menuBar.Height).MoveToY(0, 400, Easing.OutQuint);
        content.ScaleTo(.9f).ScaleTo(1f, 800, Easing.OutElastic);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        skinManager.CanChangeSkin = true;

        this.FadeOut(200);
        menuBar.MoveToY(-menuBar.Height, 400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);

        return base.OnExiting(e);
    }

    private void save()
    {
        notifications.SendText("Skin Saved", "", FontAwesome6.Solid.Check);
        skinManager.UpdateAndSave(skinJson);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.ControlPressed)
        {
            switch (e.Key)
            {
                case Key.S:
                    save();
                    return true;
            }
        }

        return true;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}
