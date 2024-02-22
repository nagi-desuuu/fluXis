using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Clubs;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User.Sections;
using fluXis.Game.Overlay.User.Sidebar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.User;

public partial class UserProfileOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private Fluxel fluxel { get; set; }

    private APIUser user;
    private Container content;
    private FluXisScrollContainer scroll;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                Width = 1360,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Masking = true,
                    EdgeEffect = FluXisStyles.ShadowLargeNoOffset,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background1
                        },
                        scroll = new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = false,
                            Child = flow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Top = 70, Bottom = 20, Horizontal = 20 },
                                Spacing = new Vector2(20)
                            }
                        }
                    }
                }
            }
        };
    }

    public void ShowUser(int id)
    {
        Show();
        if (user?.ID != id) fetch(id);
    }

    private async void fetch(int id)
    {
        user = await UserCache.GetUserAsync(id);
        if (user == null) return;

        var mapsReq = new UserMapsRequest(id);
        await mapsReq.PerformAsync(fluxel);

        Schedule(() => displayData(user, mapsReq.Response.Data));
    }

    private void displayData(APIUser user, APIUserMaps maps)
    {
        flow.Clear();
        flow.Children = new Drawable[]
        {
            new ProfileHeader(user),
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Horizontal = 10, Bottom = 20 },
                Spacing = new Vector2(20),
                Children = new Drawable[]
                {
                    new ProfileStats(user),
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.Absolute, 300),
                            new Dimension(GridSizeMode.Absolute, 20),
                            new Dimension()
                        },
                        RowDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize)
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {
                                        new ProfileSidebarClub(user.Club ?? new APIClubShort())
                                        {
                                            Alpha = user.Club == null ? 0 : 1
                                        },
                                        new ProfileFollowerList()
                                    }
                                },
                                Empty(),
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(20),
                                    Children = new Drawable[]
                                    {
                                        new ProfileMapsSection(maps)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void PopIn()
    {
        content.ResizeHeightTo(0).MoveToY(1)
               .ResizeHeightTo(1, 800, Easing.OutQuint)
               .MoveToY(0, 800, Easing.OutQuint);

        scroll.ScrollTo(0, false);
        scroll.FadeOut().Delay(400).FadeIn(200);
        this.FadeIn(200);
    }

    protected override void PopOut()
    {
        content.ResizeHeightTo(0, 800, Easing.OutQuint);
        this.FadeOut(200);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back)
            return false;

        Hide();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
