using System.Net.Http;
using fluXis.Online.API.Payloads.Auth;
using fluXis.Online.API.Responses.Auth;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Auth;

public class LoginRequest : APIRequest<LoginResponse>
{
    protected override string Path => "/auth/login";
    protected override HttpMethod Method => HttpMethod.Post;

    private string username { get; }
    private string password { get; }

    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = new LoginPayload(username, password);
        req.AddRaw(json.Serialize());
        return req;
    }
}
