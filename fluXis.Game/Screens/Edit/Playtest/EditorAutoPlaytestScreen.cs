using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Replays;
using fluXis.Game.Screens.Gameplay.Replay;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.Playtest;

public partial class EditorAutoPlaytestScreen : ReplayGameplayScreen
{
    protected override double GameplayStartTime { get; }
    public override bool FadeBackToGlobalClock => false;

    private MapInfo map { get; }

    public EditorAutoPlaytestScreen(RealmMap realmMap, MapInfo info, double startTime)
        : base(realmMap, new List<IMod> { new AutoPlayMod() }, new AutoGenerator(info, realmMap.KeyCount).Generate())
    {
        GameplayStartTime = startTime;
        map = info;
    }

    protected override MapInfo LoadMap() => map;
    public override void OnDeath() => this.Exit();
    protected override void End() => this.Exit();
}
