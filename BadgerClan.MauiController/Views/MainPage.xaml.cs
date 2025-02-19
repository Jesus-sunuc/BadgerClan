using BadgerClan.MauiController.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace BadgerClan.MauiController;

public partial class MainPage : ContentPage
{
    public ObservableCollection<IEndpointItem> AllItems { get; set; }
        = new ObservableCollection<IEndpointItem>();

    private IEndpointItem? _currentItem;


    private readonly HttpClient httpClient = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        AllItems.Add(new ApiEndpoint("Local Dev", "https://localhost:7246"));
        AllItems.Add(new ApiEndpoint("Azure Bot1", "https://badgerclan-bot1-ewb6dfcncae0dnhj.westus-01.azurewebsites.net"));
        AllItems.Add(new ApiEndpoint("Azure Bot2", "https://badgerclan-bot2-csffdzdybvgfhma7.westus-01.azurewebsites.net"));

        if (AllItems.Count > 0)
        {
            _currentItem = AllItems[0];
            CurrentEndpointLabel.Text = $"Current: {_currentItem.DisplayName}";
        }
    }

    private void OnItemSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedIndex < 0) return;

        _currentItem = AllItems[picker.SelectedIndex];
        CurrentEndpointLabel.Text = $"Current: {_currentItem.DisplayName}";
    }

    private async void OnAggressiveClicked(object sender, EventArgs e)
    {
        if (_currentItem == null) return;
        await _currentItem.SetStrategyAsync("aggressive");
    }

    private async void OnDefensiveClicked(object sender, EventArgs e)
    {
        if (_currentItem == null) return;
        await _currentItem.SetStrategyAsync("defensive");
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        if (_currentItem == null) return;
        await _currentItem.SetStrategyAsync("random");
    }

    private async void OnGoToTeamPageClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TeamPage");
    }
}
