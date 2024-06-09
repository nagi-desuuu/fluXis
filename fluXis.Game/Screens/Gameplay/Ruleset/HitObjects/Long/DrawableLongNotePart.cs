using System;
using fluXis.Game.Map.Structures;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using fluXis.Game.Skinning.Bases.HitObjects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects.Long;

public partial class DrawableLongNotePart : CompositeDrawable
{
    protected HitObject Data { get; }

    public Action<double> OnJudgement { get; set; }
    public Action OnMiss { get; set; }

    protected bool Missed => Time.Current > HitTime + HitWindows.TimingFor(HitWindows.Lowest);
    protected virtual double HitTime => Data.Time;
    protected virtual HitWindows HitWindows => Screen.HitWindows;

    public bool Judged { get; private set; }

    [Resolved]
    protected HitObjectManager ObjectManager { get; private set; }

    [Resolved]
    protected SkinManager SkinManager { get; private set; }

    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    protected DrawableLongNotePart(HitObject data)
    {
        Data = data;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
    }

    public void ApplySnapColor(int idx)
    {
        var child = InternalChild as ICanHaveSnapColor;
        child?.ApplySnapColor(idx, 0);
    }

    protected override void Update()
    {
        base.Update();

        if (Missed)
            UpdateJudgement(false);
    }

    public void UpdateJudgement(bool byUser)
    {
        if (Judged)
            return;

        var offset = HitTime - Time.Current;
        CheckJudgement(byUser, offset);
    }

    protected void CheckJudgement(bool byUser, double offset)
    {
        if (!byUser)
        {
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            OnMiss?.Invoke();
            return;
        }

        if (!HitWindows.CanBeHit(offset))
        {
            ApplyResult(HitWindows.TimingFor(HitWindows.Lowest));
            OnMiss?.Invoke();
            return;
        }

        ApplyResult(offset);
    }

    protected void ApplyResult(double offset)
    {
        if (Judged)
            return;

        Judged = true;
        OnJudgement?.Invoke(offset);
    }
}
