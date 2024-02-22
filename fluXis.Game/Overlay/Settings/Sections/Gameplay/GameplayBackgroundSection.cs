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

public partial class GameplayBackgroundSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Background;
    public override IconUsage Icon => FontAwesome6.Solid.Image;

    private SettingsGameplayStrings strings => LocalizationStrings.Settings.Gameplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = strings.BackgroundDim,
                Description = strings.BackgroundDimDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.BackgroundDim),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = strings.BackgroundBlur,
                Description = strings.BackgroundBlurDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.BackgroundBlur),
                DisplayAsPercentage = true
            },
            new SettingsToggle
            {
                Label = strings.BackgroundPulse,
                Description = strings.BackgroundPulseDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.BackgroundPulse)
            }
        });
    }
}
