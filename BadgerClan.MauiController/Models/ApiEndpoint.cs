using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BadgerClan.MauiController.Models;

public class ApiEndpoint : IEndpointItem
{
    public string Nickname { get; set; }
    public string BaseUrl { get; set; }

    public bool IsSelected { get; set; }

    public ApiEndpoint(string nickname, string baseUrl)
    {
        Nickname = nickname;
        BaseUrl = baseUrl;
    }

    public string DisplayName => Nickname;

    public async Task SetStrategyAsync(string strategyName)
    {
        using var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        var response = await client.PostAsJsonAsync("/api/strategy", strategyName);
        response.EnsureSuccessStatusCode();
    }
}