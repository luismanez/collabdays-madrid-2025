using Abyx.SharePoint.Lists.Ingestor.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;

namespace Abyx.SharePoint.Lists.Ingestor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddOptions<GraphOptions>()
            .Bind(configuration.GetSection(GraphOptions.SettingsSectionName));

        return services;
    }

    public static IServiceCollection AddKernelMemory(
        this IServiceCollection services, IConfiguration configuration)
    {
        var azureOpenAITextConfig = new AzureOpenAIConfig();
        var azureOpenAIEmbeddingConfig = new AzureOpenAIConfig();
        var azureAISearchConfig = new AzureAISearchConfig();

        configuration
            .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAITextConfig)
            .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAIEmbeddingConfig)
            .BindSection("KernelMemory:Services:AzureAISearch", azureAISearchConfig);

        var memory = new KernelMemoryBuilder()
                        .With(new KernelMemoryConfig { DefaultIndexName = "abyx-products" })
                        .WithAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingConfig)
                        .WithAzureOpenAITextGeneration(azureOpenAITextConfig)
                        .WithAzureAISearchMemoryDb(azureAISearchConfig)
                        .WithSimpleFileStorage(new SimpleFileStorageConfig
                        {
                            Directory = "/Users/luisman/temp/km-chunk-storage",
                            StorageType = FileSystemTypes.Disk
                        })
                        .Build<MemoryServerless>();

        services.AddSingleton(memory);

        return services;
    }
}
