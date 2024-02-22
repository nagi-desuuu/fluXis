using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayMapSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Map;
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.Hitsounds,
                Description = strings.HitsoundsDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.Hitsounding)
            },
            new SettingsToggle
            {
                Label = strings.BackgroundVideo,
                Description = strings.BackgroundVideoDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.BackgroundVideo)
            },
        });
    }
}
