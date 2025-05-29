using FastEndpoints;
using Abyx.Ai.Api.Extensions;
using Abyx.Ai.Api.Plugins.Native;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace Abyx.Ai.Api.Features.Chat.GetChatCompletions;


public class GetChatCompletionsEndpoint : Endpoint<GetChatCompletionsRequest, GetChatCompletionsResponse>
{
    private readonly Kernel _kernel;
    private readonly MemoryServerless _kernelMemory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetChatCompletionsEndpoint> _logger;

    public GetChatCompletionsEndpoint(
        Kernel kernel,
        MemoryServerless kernelMemory,
        HttpClient httpClient,
        ILogger<GetChatCompletionsEndpoint> logger)
    {
        _kernelMemory = kernelMemory;
        _httpClient = httpClient;
        _logger = logger;
        _kernel = kernel;
    }

    public override void Configure()
    {
        Post("/api/chat");
        AllowAnonymous();

        // Add Swagger documentation
        Summary(s =>
        {
            s.Summary = "Get AI chat completions about our products";
            s.Description = "Sends a chat request to the AI model and returns a response with products information";
            s.ExampleRequest = new GetChatCompletionsRequest { Input = "What products do you have in the Technology category?" };
            s.Response(200, "Successfully processed the chat request", example: new GetChatCompletionsResponse
            {
                UserQuery = "What products do you have in the Technology category?",
                ChatAnswer = "We have several products in the Technology category, including laptops, smartphones, and tablets."
            });
        });

        // Add tags for better organization in Swagger UI
        Tags("Chat");
    }

    public override async Task HandleAsync(GetChatCompletionsRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Received chat request: {Input}", req.Input);

        var librarianPluginYaml = EmbeddedResource.Read("OurProducts.yaml");
        var librarianPluginAsFunction = _kernel.CreateFunctionFromPromptYaml(librarianPluginYaml);
        _kernel.ImportPluginFromFunctions("OurProductsPlugin", [librarianPluginAsFunction]);

        var kernelMemoryPlugin = new MemoryPlugin(_kernelMemory);

        var ipPlugin = new MyIpAddressPlugin(_httpClient);

        _kernel.ImportPluginFromObject(kernelMemoryPlugin, "memory");
        _kernel.ImportPluginFromObject(ipPlugin);

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        var result = await _kernel.InvokePromptAsync<string>(
            req.Input,
            new(settings),
            cancellationToken: ct);

        var response = new GetChatCompletionsResponse
        {
            UserQuery = req.Input,
            ChatAnswer = result!
        };

        await SendAsync(response, cancellation: ct);
    }
}
