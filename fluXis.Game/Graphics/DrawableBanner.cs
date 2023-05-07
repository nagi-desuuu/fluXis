using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics;

public partial class DrawableBanner : Sprite
{
    private APIUserShort user;
    private TextureStore textures;

    public DrawableBanner(APIUserShort user)
    {
        this.user = user ?? APIUserShort.Dummy;
        Alpha = 0;
        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        this.textures = textures;
        Texture = textures.Get(user.BannerUrl);
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(200);
    }

    public void UpdateUser(APIUserShort newUser)
    {
        user = newUser ?? APIUserShort.Dummy;
        Texture = textures.Get(user.BannerUrl);
    }
}