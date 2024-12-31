using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Users;

public class UserScoresRequest : APIRequest<APIUserScores>
{
    protected override string Path => $"/user/{id}/scores";

    private long id { get; }

    public UserScoresRequest(long id)
    {
        this.id = id;
    }
}
