using Abyx.Ai.Api.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddFastEndpoints()
   .SwaggerDocument(options =>
   {
       options.DocumentSettings = s =>
       {
           s.Title = "Abyx AI Our Products API";
           s.Version = "v1";
           s.Description = "API for AI chat completions with product information and memory capabilities";
       };
   });

builder.Services.AddHttpClient();

builder.Services.AddSemanticKernelWithChatCompletions(builder.Configuration);
builder.Services.AddKernelMemory(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();