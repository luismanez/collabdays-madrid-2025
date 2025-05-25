using System;

namespace Abyx.Ai.Api.Features.Chat.GetChatCompletions;

public class GetChatCompletionsResponse
{
    public string ChatAnswer { get; set; } = default!;
    public string UserQuery { get; set; } = default!;
}
