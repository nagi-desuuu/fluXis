using fluXis.Graphics.Background;
using osu.Framework.Allocation;

namespace fluXis.Tests.Graphics;

public partial class TestGlobalBackground : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyAmplitude();

        var stack = new GlobalBackground();
        Add(stack);

        AddSliderStep("Set parallax", 0f, 1f, 0f, v => stack.ParallaxStrength = v);
        AddSliderStep("Set zoom", 1f, 2f, 1f, v => stack.Zoom = v);
        AddSliderStep("Set blur", 0f, 1f, 0f, v => stack.SetBlur(v));
        AddSliderStep("Set dim", 0f, 1f, 0f, v => stack.SetDim(v));
    }
}
