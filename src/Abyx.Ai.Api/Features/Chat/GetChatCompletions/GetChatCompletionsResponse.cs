using System;

namespace Abyx.Ai.Api.Features.Chat.GetChatCompletions;

public class GetChatCompletionsResponse
{
    /// <summary>
    /// The AI-generated response to the user's query
    /// </summary>
    /// <example>We have several products in the Technology category, including laptops, smartphones, and tablets.</example>
    public string ChatAnswer { get; set; } = default!;

    /// <summary>
    /// The original user query that was processed
    /// </summary>
    /// <example>What products do you have in the Technology category?</example>
    public string UserQuery { get; set; } = default!;
}
