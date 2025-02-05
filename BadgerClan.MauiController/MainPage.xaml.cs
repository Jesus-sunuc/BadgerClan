using System.Net.Http.Json;

namespace BadgerClan.MauiController;

public partial class MainPage : ContentPage
{
    public record ApiEndpoint(string Nickname, string BaseUrl);
    public List<ApiEndpoint> AvailableEndpoints { get; } = new()
    {
        new ApiEndpoint("Local Dev",    "https://localhost:7246"),
        new ApiEndpoint("Azure Bot1",   "https://badgerclan-bot1-ewb6dfcncae0dnhj.westus-01.azurewebsites.net"),
        new ApiEndpoint("Azure Bot2",   "https://badgerclan-bot2-csffdzdybvgfhma7.westus-01.azurewebsites.net"),
    };

    private ApiEndpoint? _currentEndpoint;

    private readonly HttpClient httpClient = new();


    public MainPage()
    {
        InitializeComponent();

        BindingContext = this;

        _currentEndpoint = AvailableEndpoints.First();
        httpClient.BaseAddress = new Uri(_currentEndpoint.BaseUrl);

        CurrentEndpointLabel.Text = $"Current Endpoint: {_currentEndpoint.Nickname}";

        _ = GetCurrentStrategy();
    }

    private void OnEndpointSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedIndex < 0)
            return;

        _currentEndpoint = AvailableEndpoints[picker.SelectedIndex];
        CurrentEndpointLabel.Text = $"Current Endpoint: {_currentEndpoint.Nickname}";

        _ = GetCurrentStrategy();
    }

    private async Task GetCurrentStrategy()
    {
        if (_currentEndpoint == null) return;
        try
        {
            var url = $"{_currentEndpoint.BaseUrl}/api/strategy/current";
            var current = await httpClient.GetStringAsync(url);
            CurrentStrategyLabel.Text = $"Current Strategy: {current}";
        }
        catch (Exception ex)
        {
            CurrentStrategyLabel.Text = "Failed to get current strategy: " + ex.Message;
        }
    }

    private async Task SetStrategy(string strategyName)
    {
        if (_currentEndpoint == null) return;

        try
        {
            var url = $"{_currentEndpoint.BaseUrl}/api/strategy";
            var response = await httpClient.PostAsJsonAsync(url, strategyName);
            response.EnsureSuccessStatusCode();
            await GetCurrentStrategy();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
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
}
