using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceSkinSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Skin;
    public override IconUsage Icon => FontAwesome6.Solid.PaintBrush;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [Resolved]
    private SkinManager skinManager { get; set; }

    private SettingsDropdown<string> currentDropdown;
    private BindableBool buttonsEnabled;

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager, FluXisGameBase gameBase)
    {
        buttonsEnabled = new BindableBool(true);

        AddRange(new Drawable[]
        {
            currentDropdown = new SettingsDropdown<string>
            {
                Label = strings.SkinCurrent,
                Description = strings.SkinCurrentDescription,
                Bindable = Config.GetBindable<string>(FluXisSetting.SkinName),
                Items = skinManager.GetSkinNames()
            },
            new SettingsButton
            {
                Label = strings.SkinRefresh,
                Description = strings.SkinRefreshDescription,
                ButtonText = "Refresh",
                Action = reloadList
            },
            new SettingsButton
            {
                Label = strings.SkinOpenEditor,
                Description = strings.SkinOpenEditorDescription,
                ButtonText = "Open",
                Action = gameBase.OpenSkinEditor,
                EnabledBindable = buttonsEnabled
            },
            new SettingsButton
            {
                Label = strings.SkinOpenFolder,
                Description = strings.SkinOpenFolderDescription,
                Action = skinManager.OpenFolder,
                ButtonText = "Open",
                EnabledBindable = buttonsEnabled
            },
            new SettingsButton
            {
                Label = strings.SkinExport,
                Description = strings.SkinExportDescription,
                ButtonText = "Export",
                EnabledBindable = buttonsEnabled,
                Action = skinManager.ExportCurrent
            },
            new SettingsButton
            {
                Label = strings.SkinDelete,
                Description = strings.SkinDeleteDescription,
                ButtonText = "Delete",
                EnabledBindable = buttonsEnabled,
                Action = () =>
                {
                    if (skinManager.IsDefault)
                        return;

                    gameBase.Overlay = new ConfirmDeletionPanel(() => skinManager.Delete(skinManager.SkinFolder), itemName: "skin");
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        buttonsEnabled.Value = !skinManager.IsDefault;

        skinManager.SkinChanged += () => buttonsEnabled.Value = !skinManager.IsDefault;
        skinManager.SkinListChanged += reloadList;
    }

    private void reloadList() => currentDropdown.Items = skinManager.GetSkinNames();
}
