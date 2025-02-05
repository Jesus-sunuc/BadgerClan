using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BadgerClan.MauiController.Models;
public class ApiTeam : IEndpointItem
{
    public string TeamName { get; set; }
    public List<ApiEndpoint> Endpoints { get; set; } = new();

    public ApiTeam(string name)
    {
        TeamName = name;
    }

    public string DisplayName => $"TEAM: {TeamName}";

    public async Task SetStrategyAsync(string strategyName)
    {
        foreach (var endpoint in Endpoints)
        {
            using var client = new HttpClient { BaseAddress = new Uri(endpoint.BaseUrl) };
            var response = await client.PostAsJsonAsync("/api/strategy", strategyName);
            response.EnsureSuccessStatusCode();
        }
    }
}