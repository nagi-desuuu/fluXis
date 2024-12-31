using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Panel.Types;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace fluXis.Graphics.UserInterface.Panel.Presets;

public partial class ExternalLinkPanel : ButtonPanel
{
    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private Clipboard clipboard { get; set; }

    public ExternalLinkPanel(string link)
    {
        Icon = FontAwesome6.Solid.Link;
        Text = "Just to make sure...";
        SubText = $"You're about to open the following link in your browser:\n{link}";
        Buttons = new ButtonData[]
        {
            new PrimaryButtonData(() => host.OpenUrlExternally(link)),
            new SecondaryButtonData("Copy it instead.", () => clipboard.SetText(link)),
            new CancelButtonData()
        };
    }
}
