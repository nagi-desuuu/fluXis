using fluXis.Configuration;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Screens.Gameplay;

public partial class GameplaySamples : Component
{
    private Bindable<double> hitSoundVolume;

    public Sample HitSample { get; private set; }
    private Sample restartSample;
    private Sample[] missSamples;
    private Sample failSample;

    private SampleChannel failChannel;

    [BackgroundDependencyLoader]
    private void load(SkinManager skins, FluXisConfig config)
    {
        hitSoundVolume = config.GetBindable<double>(FluXisSetting.HitSoundVolume);

        HitSample = skins.GetHitSample();
        restartSample = skins.GetRestartSample();
        missSamples = skins.GetMissSamples();
        failSample = skins.GetFailSample();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        hitSoundVolume.BindValueChanged(_ => HitSample.Volume.Value = hitSoundVolume.Value, true);
    }

    public void Hit() => HitSample?.Play();
    public void Restart() => restartSample?.Play();
    public void Miss() => missSamples?[RNG.Next(0, missSamples.Length)]?.Play();

    public void Fail() => failChannel = failSample?.Play();
    public void CancelFail() => failChannel?.Stop();
}
