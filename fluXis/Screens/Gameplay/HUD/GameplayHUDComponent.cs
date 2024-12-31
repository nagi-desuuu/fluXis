using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.HUD;

public partial class GameplayHUDComponent : Container
{
    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    public JudgementProcessor JudgementProcessor { get; set; }
    public HealthProcessor HealthProcessor { get; set; }
    public ScoreProcessor ScoreProcessor { get; set; }

    public HUDComponentSettings Settings { get; set; } = new();
}
