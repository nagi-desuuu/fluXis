using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;
using fluXis.Game.Skinning;
using fluXis.Game.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Playfield : Container
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private GameplayScreen screen { get; set; }

    public FillFlowContainer<Receptor> Receptors { get; private set; }
    public HitObjectManager Manager { get; private set; }
    public Stage Stage { get; private set; }

    public MapInfo Map => screen.Map;
    public RealmMap RealmMap => screen.RealmMap;

    private DependencyContainer dependencies;

    private TimingLineManager timingLineManager;

    private Drawable topCover;
    private Drawable bottomCover;

    private Bindable<float> topCoverHeight;
    private Bindable<float> bottomCoverHeight;
    private Bindable<ScrollDirection> scrollDirection;
    private Bindable<double> hitsoundPanStrength;

    public bool IsUpScroll => scrollDirection.Value == ScrollDirection.Up;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;

        topCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverTop);
        bottomCoverHeight = config.GetBindable<float>(FluXisSetting.LaneCoverBottom);
        scrollDirection = config.GetBindable<ScrollDirection>(FluXisSetting.ScrollDirection);
        hitsoundPanStrength = config.GetBindable<double>(FluXisSetting.HitsoundPanning);

        dependencies.CacheAs(Manager = new HitObjectManager
        {
            AlwaysPresent = true,
            Masking = true
        });

        var playfieldEvents = new List<IApplicableToPlayfield>();
        playfieldEvents.AddRange(screen.MapEvents.PlayfieldMoveEvents);
        playfieldEvents.AddRange(screen.MapEvents.PlayfieldScaleEvents);
        playfieldEvents.AddRange(screen.MapEvents.PlayfieldFadeEvents);
        playfieldEvents.AddRange(screen.MapEvents.PlayfieldRotateEvents);

        InternalChildren = new[]
        {
            Stage = new Stage(),
            timingLineManager = new TimingLineManager(),
            Manager,
            Receptors = new FillFlowContainer<Receptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                ChildrenEnumerable = Enumerable.Range(0, RealmMap.KeyCount).Select(i => new Receptor(i))
            },
            skinManager.GetHitLine().With(d =>
            {
                d.Width = 1;
                d.RelativeSizeAxes = Axes.X;
                d.Y = -skinManager.SkinJson.GetKeymode(RealmMap.KeyCount).HitPosition;
            }),
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                Children = new[]
                {
                    topCover = skinManager.GetLaneCover(false),
                    bottomCover = skinManager.GetLaneCover(true)
                }
            },
            new AnimatedEventHandler<IApplicableToPlayfield>(playfieldEvents),
            new EventHandler<ShakeEvent>(screen.MapEvents.ShakeEvents, shake => screen.Shake(shake.Duration, shake.Magnitude)),
            new EventHandler<HitObjectFadeEvent>(screen.MapEvents.HitObjectFadeEvents, fade => Manager.FadeTo(fade.Alpha, fade.Duration, fade.Easing))
        };
    }

    protected override void LoadComplete()
    {
        timingLineManager.CreateLines(Map);
        base.LoadComplete();

        scrollDirection.BindValueChanged(_ =>
        {
            if (IsUpScroll)
                Scale *= new Vector2(1, -1);
        }, true);
    }

    protected override void Update()
    {
        topCover.Y = (topCoverHeight.Value - 1f) / 2f;
        bottomCover.Y = (1f - bottomCoverHeight.Value) / 2f;

        var screenWidth = screen.DrawWidth;
        var relativePos = (X + screenWidth / 2) / screenWidth;
        screen.Hitsounding.PlayfieldPanning.Value = Math.Clamp(relativePos * 2 - 1, -1, 1) * hitsoundPanStrength.Value;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
