﻿using System.Net.Http;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Payloads.Chat;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Chat;

public class ChatMessageRequest : APIRequest<IChatMessage>
{
    protected override string Path => $"/chat/channels/{channel}/messages";
    protected override HttpMethod Method => HttpMethod.Post;

    private string channel { get; }
    private string content { get; }

    public ChatMessageRequest(string channel, string content)
    {
        this.channel = channel;
        this.content = content;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new ChatMessagePayload(content).Serialize());
        return req;
    }
}
