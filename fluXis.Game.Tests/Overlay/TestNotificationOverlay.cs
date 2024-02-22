using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestNotificationOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        Add(notifications.Floating = new FloatingNotificationContainer());
        Add(notifications.Tasks = new TaskNotificationContainer());

        int count = 0;

        AddStep("Send notification", () => notifications.SendText("This is a test notification"));
        AddStep("Send counting notification", () => notifications.SendText("This is a test notification", $"Count: {++count}", FontAwesome6.Solid.Circle));
        AddStep("Send error notification", () => notifications.SendError("This is a test error notification"));
    }
}
