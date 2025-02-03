using BadgerClan.Logic;
using BadgerClan.Logic.Bot;
using Microsoft.AspNetCore.Mvc;

namespace BadgerClan.ControlledBotApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapPost("/", async (MoveRequest request) =>
        {
            var currentTeam = new Team(request.YourTeamId)
            {
                Medpacs = request.Medpacs
            };

            var gameState = new GameState(
                request.GameId,
                request.BoardSize,
                request.TurnNumber,
                request.Units.Select(FromDto),
                request.TeamIds,
                currentTeam
            );

            var moves = await StrategyState.CurrentBot.PlanMovesAsync(gameState);

            return new MoveResponse(moves);
        });

        app.MapPost("/api/strategy", ([FromBody] string strategyName) =>
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

        app.MapGet("/api/strategy/current", () =>
        {
            return StrategyState.CurrentBot.GetType().Name;
        });

        app.Run();
    }

    private static Unit FromDto(UnitDto dto)
    {
        return Unit.Factory(
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
}
