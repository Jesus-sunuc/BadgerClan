using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using BadgerClan.GrpcBotApi.Services; // For BotStrategyService
using BadgerClan.Logic;
using BadgerClan.Logic.Bot;
using BadgerClan.GrpcBotApi;

var builder = WebApplication.CreateBuilder(args);

// Add the gRPC services
builder.Services.AddGrpc();

var app = builder.Build();

// 1) This maps your gRPC service from the .proto
app.MapGrpcService<BotStrategyService>();

// 2) This maps a REST/JSON endpoint so your existing NetworkBot can POST a MoveRequest
//    without causing JSON parse errors.
app.MapPost("/", async (BadgerClan.Logic.MoveRequest request) =>
{
    // Build the GameState from the request (fully qualifying the types).
    var currentTeam = new Team(request.YourTeamId) { Medpacs = request.Medpacs };
    var gameState = new GameState(
        request.GameId,
        request.BoardSize,
        request.TurnNumber,
        request.Units.Select(FromDto),
        request.TeamIds,
        currentTeam
    );

    // Use a static StrategyState if you want dynamic strategies
    IBot bot = StrategyState.CurrentBot;

    var moves = await bot.PlanMovesAsync(gameState);
    return new MoveResponse(moves);
});

// (Optional) Provide a REST endpoint to change the strategy if you want
app.MapPost("/api/strategy", (string strategyName) =>
{
    switch (strategyName.ToLowerInvariant())
    {
        case "aggressive":
            StrategyState.CurrentBot = new AggressiveBot();
            break;
        case "defensive":
            StrategyState.CurrentBot = new DefensiveBot();
            break;
        case "random":
            StrategyState.CurrentBot = new RandomBot();
            break;
        default:
            StrategyState.CurrentBot = new NothingBot();
            break;
    }
    return Results.Ok($"Strategy changed to {strategyName}");
});

// (Optional) Provide a REST endpoint to GET the current strategy
app.MapGet("/api/strategy/current", () =>
{
    return StrategyState.CurrentBot.GetType().Name;
});

app.Run();

// Helper to convert BadgerClan.Logic.UnitDto -> BadgerClan.Logic.Unit
// Note we're fully qualifying the return type to avoid confusion.
static BadgerClan.Logic.Unit FromDto(BadgerClan.Logic.UnitDto dto)
{
    return BadgerClan.Logic.Unit.Factory(
        dto.Type,
        dto.Id,
        dto.Attack,
        dto.AttackDistance,
        dto.Health,
        dto.MaxHealth,
        dto.Moves,
        dto.MaxMoves,
        dto.Location,
        dto.Team
    );
}
