using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Lines;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Design.Playfield;

public partial class EditorDesignPlayfield : CompositeDrawable
{
    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private LaneSwitchManager laneSwitchManager { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    public FillFlowContainer<EditorDesignReceptor> Receptors { get; private set; }
    private Drawable hitline;

    private int index { get; }

    public EditorDesignPlayfield(int index)
    {
        this.index = index;
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        AlwaysPresent = true;

        InternalChildren = new[]
        {
            new Stage(),
            new EditorTimingLines(),
            Receptors = new FillFlowContainer<EditorDesignReceptor>
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Padding = new MarginPadding { Bottom = skinManager.SkinJson.GetKeymode(map.RealmMap.KeyCount).ReceptorOffset }
            },
            hitline = skinManager.GetHitLine().With(l =>
            {
                l.RelativeSizeAxes = Axes.X;
                l.Width = 1;
            })
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        reload();
    }

    private void reload()
    {
        Receptors.Clear();
        Receptors.ChildrenEnumerable = Enumerable.Range(0, map.RealmMap.KeyCount).Select(i => new EditorDesignReceptor(i, laneSwitchManager));
    }

    protected override void Update()
    {
        base.Update();

        hitline.Y = -laneSwitchManager.HitPosition;

        updateRotation();
        updateScale();
        updatePosition();
        updateAlpha();
    }

    private bool applies(IApplicableToPlayfield ev)
    {
        if (ev.PlayfieldIndex == 0)
            return true;

        return ev.PlayfieldIndex == index + 1;
    }

    private void updateRotation()
    {
        var current = map.MapEvents.PlayfieldRotateEvents.Where(applies).LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            Rotation = 0;
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var end = current.Roll;

        if (progress >= 1)
        {
            Rotation = end;
            return;
        }

        var previous = map.MapEvents.PlayfieldRotateEvents.Where(applies).LastOrDefault(e => e.Time < current.Time);
        var start = previous?.Roll ?? 0;

        if (progress < 0)
        {
            Rotation = start;
            return;
        }

        Rotation = Interpolation.ValueAt(clock.CurrentTime, start, end, current.Time, current.Time + current.Duration, current.Easing);
    }

    private void updateAlpha()
    {
        var events = map.MapEvents.LayerFadeEvents.Where(x => x.Layer == LayerFadeEvent.FadeLayer.Playfield).Where(applies).ToList();
        var current = events.LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (current == null)
        {
            Alpha = 1;
            return;
        }

        var progress = (clock.CurrentTime - current.Time) / current.Duration;
        var end = current.Alpha;

        if (progress >= 1)
        {
            Alpha = end;
            return;
        }

        var previous = events.LastOrDefault(e => e.Time < current.Time);
        var start = previous?.Alpha ?? 1;

        if (progress < 0)
        {
            Alpha = start;
            return;
        }

        Alpha = Interpolation.ValueAt(clock.CurrentTime, start, end, current.Time, current.Time + current.Duration, current.Easing);
    }

    private void updateScale()
    {
        var curScale = map.MapEvents.PlayfieldScaleEvents.Where(applies).LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (curScale == null)
        {
            Scale = Vector2.One;
            return;
        }

        var progress = (clock.CurrentTime - curScale.Time) / curScale.Duration;
        var end = new Vector2(curScale.ScaleX, curScale.ScaleY);

        if (progress >= 1)
        {
            Scale = end;
            return;
        }

        var prevScale = map.MapEvents.PlayfieldScaleEvents.Where(applies).LastOrDefault(e => e.Time < curScale.Time);
        var prev = Vector2.One;

        if (prevScale != null)
            prev = new Vector2(prevScale.ScaleX, prevScale.ScaleY);

        if (progress < 0)
        {
            Scale = prev;
            return;
        }

        Scale = Interpolation.ValueAt(clock.CurrentTime, prev, end, curScale.Time, curScale.Time + curScale.Duration, curScale.Easing);
    }

    private void updatePosition()
    {
        var curMove = map.MapEvents.PlayfieldMoveEvents.Where(applies).LastOrDefault(e => e.Time <= clock.CurrentTime);

        if (curMove == null)
        {
            Position = Vector2.One;
            return;
        }

        var progress = (clock.CurrentTime - curMove.Time) / curMove.Duration;
        var end = new Vector2(curMove.OffsetX, curMove.OffsetY);

        if (progress >= 1)
        {
            Position = end;
            return;
        }

        var prevMove = map.MapEvents.PlayfieldMoveEvents.Where(applies).LastOrDefault(e => e.Time < curMove.Time);
        var prev = Vector2.One;

        if (prevMove != null)
            prev = new Vector2(prevMove.OffsetX, prevMove.OffsetY);

        if (progress < 0)
        {
            Position = prev;
            return;
        }

        Position = Interpolation.ValueAt(clock.CurrentTime, prev, end, curMove.Time, curMove.Time + curMove.Duration, curMove.Easing);
    }
}
