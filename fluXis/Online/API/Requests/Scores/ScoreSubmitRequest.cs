using System.Net.Http;
using fluXis.Online.API.Responses.Scores;
using fluXis.Scoring;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<ScoreSubmissionStats>
{
    protected override string Path => "/scores";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }

    public ScoreSubmitRequest(ScoreInfo score)
    {
        this.score = score;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(score.Serialize());
        return req;
    }
}
