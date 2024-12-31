using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Graphics.Shaders.Vignette;

public partial class VignetteContainer : ShaderContainer
{
    protected override string FragmentShader => "Vignette";
    public override ShaderType Type => ShaderType.Vignette;
    protected override DrawNode CreateShaderDrawNode() => new VignetteContainerDrawNode(this, SharedData);
}
