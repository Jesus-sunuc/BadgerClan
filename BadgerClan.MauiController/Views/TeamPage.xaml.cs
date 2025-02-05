using BadgerClan.MauiController.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace BadgerClan.MauiController.Views;

public partial class TeamPage : ContentPage
{
    public List<ApiEndpoint> AvailableEndpoints { get; } = new()
    {
        new ApiEndpoint("Local Dev",    "https://localhost:7246"),
        new ApiEndpoint("Azure Bot1",   "https://badgerclan-bot1.azurewebsites.net"),
        new ApiEndpoint("Azure Bot2",   "https://badgerclan-bot2.azurewebsites.net"),
    };

    public ObservableCollection<ApiTeam> ApiTeams { get; } = new();

    public TeamPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void OnCreateTeamClicked(object sender, EventArgs e)
    {
        var teamName = TeamNameEntry.Text?.Trim();
        if (string.IsNullOrEmpty(teamName)) return;

        var selectedEndpoints = AvailableEndpoints.Where(ep => ep.IsSelected).ToList();
        if (!selectedEndpoints.Any()) return;

        var existingTeam = ApiTeams.FirstOrDefault(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase));

        if (existingTeam == null)
        {
            var newTeam = new ApiTeam(teamName);
            newTeam.Endpoints.AddRange(selectedEndpoints);
            ApiTeams.Add(newTeam);
        }
        else
        {
            existingTeam.Endpoints.Clear();
            existingTeam.Endpoints.AddRange(selectedEndpoints);
        }

        foreach (var ep in AvailableEndpoints)
        {
            ep.IsSelected = false;
        }

        TeamNameEntry.Text = string.Empty;

        EndpointListView.ItemsSource = null;
        EndpointListView.ItemsSource = AvailableEndpoints;

        TeamListView.ItemsSource = null;
        TeamListView.ItemsSource = ApiTeams;
    }
}
