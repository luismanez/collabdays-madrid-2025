using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Abyx.Ai.Api.Plugins.Native;

public class MyIpAddressPlugin
{
    private readonly HttpClient _client;

    public MyIpAddressPlugin(HttpClient client)
    {
        _client = client;
    }

    [KernelFunction, Description("Get your IP address")]
    public async Task<string> WhatsMyIp()
    {
        var data = await _client.GetStringAsync("https://api.ipify.org");
        return data;
    }
}
