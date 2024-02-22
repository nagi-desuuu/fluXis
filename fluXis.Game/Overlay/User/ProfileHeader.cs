using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Drawables;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User.Header;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.User;

public partial class ProfileHeader : Container
{
    private APIUser user { get; }

    public ProfileHeader(APIUser user)
    {
        this.user = user;
    }

    [BackgroundDependencyLoader]
    private void load(Fluxel fluxel)
    {
        RelativeSizeAxes = Axes.X;
        Height = 440;
        CornerRadius = 20;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new LoadWrapper<DrawableBanner>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableBanner(user)
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = banner => banner.FadeInFromZero(400)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2.Opacity(.5f)
            },
            new FillFlowContainer
            {
                Width = 1200,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 120,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 120),
                            new Dimension(GridSizeMode.Absolute, 10),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                new LoadWrapper<DrawableAvatar>
                                {
                                    Size = new Vector2(120),
                                    CornerRadius = 20,
                                    Masking = true,
                                    EdgeEffect = FluXisStyles.ShadowMedium,
                                    LoadContent = () => new DrawableAvatar(user)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        FillMode = FillMode.Fill,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre
                                    },
                                    OnComplete = avatar => avatar.FadeInFromZero(400)
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Children = new Drawable[]
                                    {
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Children = new[]
                                            {
                                                new HeaderRoleChip
                                                {
                                                    RoleId = user.Role,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                },
                                                getOnlineStatus().With(d =>
                                                {
                                                    d.Anchor = Anchor.CentreRight;
                                                    d.Origin = Anchor.CentreRight;
                                                })
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Horizontal,
                                            Spacing = new Vector2(10),
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = user.GetName(),
                                                    WebFontSize = 48,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Text = user.Username,
                                                    WebFontSize = 24,
                                                    Alpha = string.IsNullOrEmpty(user.DisplayName) ? 0 : .8f,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Children = new Drawable[]
                                {
                                    new HeaderPlacementChip
                                    {
                                        Placement = user.GlobalRank,
                                        CreateIcon = () => new SpriteIcon
                                        {
                                            Icon = FontAwesome6.Solid.EarthAmericas,
                                            Size = new Vector2(20)
                                        }
                                    },
                                    new HeaderPlacementChip
                                    {
                                        Placement = user.CountryRank,
                                        CreateIcon = () => new DrawableCountry(user.Country)
                                        {
                                            Size = new Vector2(20)
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(10),
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Children = new Drawable[]
                                {
                                    new HeaderButton
                                    {
                                        Icon = FontAwesome6.Solid.ShareNodes
                                    },
                                    new HeaderButton
                                    {
                                        Icon = FontAwesome6.Solid.UserPlus,
                                        Alpha = user.ID == fluxel.LoggedInUser?.ID ? 0 : 1,
                                        Text = "Follow"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private Drawable getOnlineStatus()
    {
        if (user.Online)
        {
            return new FluXisSpriteText
            {
                Text = "Currently Online",
                WebFontSize = 16
            };
        }

        return new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(5),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Last Online",
                    WebFontSize = 16,
                    Alpha = .8f
                },
                new FluXisSpriteText
                {
                    Text = TimeUtils.Ago(TimeUtils.GetFromSeconds(user.LastLogin)),
                    WebFontSize = 16
                }
            }
        };
    }
}
