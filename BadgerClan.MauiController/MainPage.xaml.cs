using BadgerClan.GrpcBotApi;
using System.Net.Http.Json;

namespace BadgerClan.MauiController;

public partial class MainPage : ContentPage
{
    public record ApiEndpoint(string Nickname, string BaseUrl, EndpointType Type);

    public enum EndpointType
    {
        Rest,
        Grpc
    }
    public List<ApiEndpoint> AvailableEndpoints { get; } = new()
    {
        // Two REST endpoints
        new ApiEndpoint("Local Dev (REST)",  "https://localhost:7246", EndpointType.Rest),
        new ApiEndpoint("Azure Bot1 (REST)", "https://badgerclan-bot1-ewb6dfcncae0dnhj.westus-01.azurewebsites.net", EndpointType.Rest),
        new ApiEndpoint("Azure Bot2 (REST)", "https://badgerclan-bot2-csffdzdybvgfhma7.westus-01.azurewebsites.net", EndpointType.Rest),

        // One gRPC endpoint
        new ApiEndpoint("Azure GRPC Bot", "https://badgerclan-grpcbot1-e5a9f7huhhddd6gm.westus-01.azurewebsites.net", EndpointType.Grpc),
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
            if (_currentEndpoint.Type == EndpointType.Rest)
            {
                var url = $"{_currentEndpoint.BaseUrl}/api/strategy/current";
                var current = await httpClient.GetStringAsync(url);
                CurrentStrategyLabel.Text = $"Current Strategy: {current}";
            }
            else if (_currentEndpoint.Type == EndpointType.Grpc)
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(_currentEndpoint.BaseUrl);
                var client = new BotStrategy.BotStrategyClient(channel);

                var reply = await client.GetCurrentStrategyAsync(new GetCurrentStrategyRequest());
                CurrentStrategyLabel.Text = $"Current Strategy: {reply.CurrentStrategy}";
            }
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
            if (_currentEndpoint.Type == EndpointType.Rest)
            {
                var url = $"{_currentEndpoint.BaseUrl}/api/strategy";
                var response = await httpClient.PostAsJsonAsync(url, strategyName);
                response.EnsureSuccessStatusCode();
            }
            else if (_currentEndpoint.Type == EndpointType.Grpc)
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(_currentEndpoint.BaseUrl);
                var client = new BotStrategy.BotStrategyClient(channel);

                var request = new SetStrategyRequest { StrategyName = strategyName };
                var reply = await client.SetStrategyAsync(request);
            }

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
