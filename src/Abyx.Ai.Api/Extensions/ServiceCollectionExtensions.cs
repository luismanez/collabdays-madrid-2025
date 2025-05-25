using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.SemanticKernel;

namespace Abyx.Ai.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelWithChatCompletions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var azureOpenAiTextConfig = new AzureOpenAIConfig();
        configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig); // re-using KM config (for simplicity)

        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                azureOpenAiTextConfig.Deployment,
                azureOpenAiTextConfig.Endpoint,
                azureOpenAiTextConfig.APIKey,
                httpClient: factory.CreateClient()); // workaround for tracing requests using Fiddler

            var kernel = builder.Build();
            return kernel;
        });

        return services;
    }

    public static IServiceCollection AddKernelMemory(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var azureOpenAiTextConfig = new AzureOpenAIConfig();
            var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig();
            var azureAiSearchConfig = new AzureAISearchConfig();
            var kernelMemoryConfig = new KernelMemoryConfig();

            configuration
                .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig)
                .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAiEmbeddingConfig)
                .BindSection("KernelMemory:Services:AzureAISearch", azureAiSearchConfig)
                .BindSection("KernelMemory", kernelMemoryConfig);

            var factory = sp.GetRequiredService<IHttpClientFactory>();

            var kmBuilder = new KernelMemoryBuilder()
                            .With(kernelMemoryConfig)
                            .WithAzureOpenAITextEmbeddingGeneration(
                                config: azureOpenAiEmbeddingConfig,
                                httpClient: factory.CreateClient(),
                                textTokenizer: new GPT4oTokenizer())
                            .WithAzureOpenAITextGeneration(
                                config: azureOpenAiTextConfig,
                                httpClient: factory.CreateClient(),
                                textTokenizer: new GPT4oTokenizer())
                            .WithAzureAISearchMemoryDb(azureAiSearchConfig);

            kmBuilder.Services.AddLogging(builder =>
            {
                builder.AddConsole();

                // builder.AddApplicationInsights(telemetryConfiguration =>
                // {
                //     // telemetryConfiguration.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                //     telemetryConfiguration.InstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
                // }, loggerOptions =>
                // {
                //     //loggerOptions.FlushOnDispose = true;
                // });
            });

            var memory = kmBuilder.Build<MemoryServerless>(
                KernelMemoryBuilderBuildOptions.WithVolatileAndPersistentData);

            return memory;
        });

        return services;
    }
}
