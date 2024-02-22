using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Input;

public partial class InputKeybindingsSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Keybindings;
    public override IconUsage Icon => FontAwesome6.Solid.Keyboard;

    private SettingsInputStrings strings => LocalizationStrings.Settings.Input;

    [Resolved]
    private MapStore mapStore { get; set; }

    private Drawable[] otherKeymodesSection => new Drawable[]
    {
        otherKeymodesTitle,
        oneKeyLayout,
        twoKeyLayout,
        threeKeyLayout,
        nineKeyLayout,
        tenKeyLayout
    };

    private KeybindSectionTitle otherKeymodesTitle;
    private SettingsKeybind oneKeyLayout;
    private SettingsKeybind twoKeyLayout;
    private SettingsKeybind threeKeyLayout;
    private SettingsKeybind nineKeyLayout;
    private SettingsKeybind tenKeyLayout;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new KeybindSectionTitle { Text = strings.Navigation },
            new SettingsKeybind
            {
                Label = strings.PreviousSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Previous }
            },
            new SettingsKeybind
            {
                Label = strings.NextSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Next }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousGroup,
                Keybinds = new object[] { FluXisGlobalKeybind.PreviousGroup }
            },
            new SettingsKeybind
            {
                Label = strings.NextGroup,
                Keybinds = new object[] { FluXisGlobalKeybind.NextGroup }
            },
            new SettingsKeybind
            {
                Label = strings.Back,
                Keybinds = new object[] { FluXisGlobalKeybind.Back }
            },
            new SettingsKeybind
            {
                Label = strings.Select,
                Keybinds = new object[] { FluXisGlobalKeybind.Select }
            },
            new KeybindSectionTitle { Text = strings.SongSelect },
            new SettingsKeybind
            {
                Label = strings.DecreaseRate,
                Keybinds = new object[] { FluXisGlobalKeybind.DecreaseRate }
            },
            new SettingsKeybind
            {
                Label = strings.IncreaseRate,
                Keybinds = new object[] { FluXisGlobalKeybind.IncreaseRate }
            },
            new KeybindSectionTitle { Text = strings.Editing },
            new SettingsKeybind
            {
                Label = strings.DeleteSelection,
                Keybinds = new object[] { FluXisGlobalKeybind.Delete }
            },
            new SettingsKeybind
            {
                Label = strings.Undo,
                Keybinds = new object[] { FluXisGlobalKeybind.Undo }
            },
            new SettingsKeybind
            {
                Label = strings.Redo,
                Keybinds = new object[] { FluXisGlobalKeybind.Redo }
            },
            new KeybindSectionTitle { Text = strings.Overlays },
            new SettingsKeybind
            {
                Label = strings.ToggleSettings,
                Keybinds = new object[] { FluXisGlobalKeybind.ToggleSettings }
            },
            new KeybindSectionTitle { Text = strings.Audio },
            new SettingsKeybind
            {
                Label = strings.VolumeDown,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeDecrease }
            },
            new SettingsKeybind
            {
                Label = strings.VolumeUp,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeIncrease }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousVolumeCategory,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumePreviousCategory }
            },
            new SettingsKeybind
            {
                Label = strings.NextVolumeCategory,
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeNextCategory }
            },
            new SettingsKeybind
            {
                Label = strings.PreviousTrack,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPrevious }
            },
            new SettingsKeybind
            {
                Label = strings.NextTrack,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicNext }
            },
            new SettingsKeybind
            {
                Label = strings.PlayPause,
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPause }
            },
            new KeybindSectionTitle { Text = strings.Keymodes },
            new SettingsKeybind
            {
                Label = strings.FourKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key4k1,
                    FluXisGameplayKeybind.Key4k2,
                    FluXisGameplayKeybind.Key4k3,
                    FluXisGameplayKeybind.Key4k4
                }
            },
            new SettingsKeybind
            {
                Label = strings.FiveKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key5k1,
                    FluXisGameplayKeybind.Key5k2,
                    FluXisGameplayKeybind.Key5k3,
                    FluXisGameplayKeybind.Key5k4,
                    FluXisGameplayKeybind.Key5k5
                }
            },
            new SettingsKeybind
            {
                Label = strings.SixKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key6k1,
                    FluXisGameplayKeybind.Key6k2,
                    FluXisGameplayKeybind.Key6k3,
                    FluXisGameplayKeybind.Key6k4,
                    FluXisGameplayKeybind.Key6k5,
                    FluXisGameplayKeybind.Key6k6
                }
            },
            new SettingsKeybind
            {
                Label = strings.SevenKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key7k1,
                    FluXisGameplayKeybind.Key7k2,
                    FluXisGameplayKeybind.Key7k3,
                    FluXisGameplayKeybind.Key7k4,
                    FluXisGameplayKeybind.Key7k5,
                    FluXisGameplayKeybind.Key7k6,
                    FluXisGameplayKeybind.Key7k7
                }
            },
            new SettingsKeybind
            {
                Label = strings.EightKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key8k1,
                    FluXisGameplayKeybind.Key8k2,
                    FluXisGameplayKeybind.Key8k3,
                    FluXisGameplayKeybind.Key8k4,
                    FluXisGameplayKeybind.Key8k5,
                    FluXisGameplayKeybind.Key8k6,
                    FluXisGameplayKeybind.Key8k7,
                    FluXisGameplayKeybind.Key8k8
                }
            },
            otherKeymodesTitle = new KeybindSectionTitle { Text = strings.OtherKeymodes },
            oneKeyLayout = new SettingsKeybind
            {
                Label = strings.OneKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key1k1
                }
            },
            twoKeyLayout = new SettingsKeybind
            {
                Label = strings.TwoKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key2k1,
                    FluXisGameplayKeybind.Key2k2
                }
            },
            threeKeyLayout = new SettingsKeybind
            {
                Label = strings.ThreeKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key3k1,
                    FluXisGameplayKeybind.Key3k2,
                    FluXisGameplayKeybind.Key3k3
                }
            },
            nineKeyLayout = new SettingsKeybind
            {
                Label = strings.NineKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key9k1,
                    FluXisGameplayKeybind.Key9k2,
                    FluXisGameplayKeybind.Key9k3,
                    FluXisGameplayKeybind.Key9k4,
                    FluXisGameplayKeybind.Key9k5,
                    FluXisGameplayKeybind.Key9k6,
                    FluXisGameplayKeybind.Key9k7,
                    FluXisGameplayKeybind.Key9k8,
                    FluXisGameplayKeybind.Key9k9
                }
            },
            tenKeyLayout = new SettingsKeybind
            {
                Label = strings.TenKey,
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key10k1,
                    FluXisGameplayKeybind.Key10k2,
                    FluXisGameplayKeybind.Key10k3,
                    FluXisGameplayKeybind.Key10k4,
                    FluXisGameplayKeybind.Key10k5,
                    FluXisGameplayKeybind.Key10k6,
                    FluXisGameplayKeybind.Key10k7,
                    FluXisGameplayKeybind.Key10k8,
                    FluXisGameplayKeybind.Key10k9,
                    FluXisGameplayKeybind.Key10k10
                }
            },
            new KeybindSectionTitle { Text = strings.InGame },
            new SettingsKeybind
            {
                Label = strings.SkipIntro,
                Keybinds = new object[] { FluXisGlobalKeybind.Skip }
            },
            new SettingsKeybind
            {
                Label = strings.DecreaseSpeed,
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedDecrease }
            },
            new SettingsKeybind
            {
                Label = strings.IncreaseSpeed,
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedIncrease }
            },
            new SettingsKeybind
            {
                Label = strings.QuickRestart,
                Keybinds = new object[] { FluXisGlobalKeybind.QuickRestart }
            },
            new SettingsKeybind
            {
                Label = strings.QuickExit,
                Keybinds = new object[] { FluXisGlobalKeybind.QuickExit }
            },
            new SettingsKeybind
            {
                Label = strings.SeekBackward,
                Keybinds = new object[] { FluXisGlobalKeybind.SeekBackward }
            },
            new SettingsKeybind
            {
                Label = strings.SeekForward,
                Keybinds = new object[] { FluXisGlobalKeybind.SeekForward }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        mapStore.CollectionUpdated += () => Schedule(updateOtherKeymodes);
        updateOtherKeymodes();
    }

    private void updateOtherKeymodes()
    {
        if (mapStore.MapSets.Any(x => x.Maps.Any(y => y.KeyCount is > 8 or < 4)))
            otherKeymodesSection.ForEach(x => x.Show());
        else
            otherKeymodesSection.ForEach(x => x.Hide());
    }

    private partial class KeybindSectionTitle : FluXisSpriteText
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            FontSize = 30;
        }
    }
}
