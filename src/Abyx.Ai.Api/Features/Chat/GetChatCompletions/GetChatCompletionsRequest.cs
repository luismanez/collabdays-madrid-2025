using System;
using System.ComponentModel.DataAnnotations;

namespace Abyx.Ai.Api.Features.Chat.GetChatCompletions;

public class GetChatCompletionsRequest
{
    /// <summary>
    /// The user input message to process
    /// </summary>
    /// <example>What products do you have in the Technology category?</example>
    [Required]
    public string Input { get; set; } = default!;
}
