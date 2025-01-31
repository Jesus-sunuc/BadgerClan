using System.Net.Http.Json;

namespace BadgerClan.MauiController;

public partial class MainPage : ContentPage
{
    private const string BotApiBaseUrl = "http://localhost:5156";

    private readonly HttpClient httpClient = new();

    public MainPage()
    {
        InitializeComponent();  
        httpClient.BaseAddress = new Uri(BotApiBaseUrl);
        _ = GetCurrentStrategy(); 
    }

    private async Task GetCurrentStrategy()
    {
        try
        {
            var current = await httpClient.GetStringAsync("/api/strategy/current");
            CurrentStrategyLabel.Text = $"Current Strategy: {current}";
        }
        catch (Exception ex)
        {
            CurrentStrategyLabel.Text = "Failed to get current strategy: " + ex.Message;
        }
    }

    private async void OnAggressiveClicked(object sender, EventArgs e)
    {
        await SetStrategy("aggressive");
    }

    private async void OnDefensiveClicked(object sender, EventArgs e)
    {
        await SetStrategy("defensive");
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        await SetStrategy("random");
    }

    private async Task SetStrategy(string strategyName)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("/api/strategy", strategyName);
            response.EnsureSuccessStatusCode();

            await DisplayAlert("Success", $"Strategy changed to {strategyName}", "OK");
            await GetCurrentStrategy();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
