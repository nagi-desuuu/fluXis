using System;
using System.Linq;
using fluXis.Desktop.Integration;
using fluXis.Game;
using fluXis.Game.Integration;
using fluXis.Game.IPC;
using fluXis.Game.Updater;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace fluXis.Desktop;

public partial class FluXisGameDesktop : FluXisGame
{
    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        var window = host.Window;
        window.Title = "fluXis " + VersionString;
        // window.ConfineMouseMode.Value = ConfineMouseMode.Never;
        window.CursorState = CursorState.Hidden;
        window.DragDrop += f => HandleDragDrop(new[] { f });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        new IPCImportChannel(Host, this);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        new DiscordActivity().Initialize(Fluxel, Activity);

        var args = Program.Args.ToList();
        args.RemoveAll(a => a.StartsWith("-"));
        HandleDragDrop(args.ToArray());
    }

    public override LightController CreateLightController() => new OpenRGBController();
    public override IUpdatePerformer CreateUpdatePerformer() => OperatingSystem.IsWindows() ? new WindowsUpdatePerformer(NotificationManager) : null;
}
