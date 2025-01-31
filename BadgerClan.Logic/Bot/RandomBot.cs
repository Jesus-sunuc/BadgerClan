using BadgerClan.Logic;

namespace BadgerClan.Logic.Bot;

public class RandomBot : IBot
{
    private readonly Random rnd = new();

    public Task<List<Move>> PlanMovesAsync(GameState state)
    {
        // Moves randomly to a neighbor, just as a silly example
        var moves = new List<Move>();

        var myTeamId = state.CurrentTeamId;
        foreach (var unit in state.Units.Where(u => u.Team == myTeamId))
        {
            var neighbors = unit.Location.Neighbors();
            if (neighbors.Any())
            {
                var randomNeighbor = neighbors[rnd.Next(neighbors.Count)];
                moves.Add(new Move(MoveType.Walk, unit.Id, randomNeighbor));
            }
        }

        return Task.FromResult(moves);
    }
}
