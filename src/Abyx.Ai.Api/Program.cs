using Abyx.Ai.Api.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddFastEndpoints()
   .SwaggerDocument();

builder.Services.AddHttpClient();

builder.Services.AddSemanticKernelWithChatCompletions(builder.Configuration);
builder.Services.AddKernelMemory(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();