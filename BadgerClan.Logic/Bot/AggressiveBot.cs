using BadgerClan.Logic;

namespace BadgerClan.Logic.Bot;

public class AggressiveBot : IBot
{
    public Task<List<Move>> PlanMovesAsync(GameState state)
    {
        var moves = new List<Move>();

        var myTeamId = state.CurrentTeamId;
        foreach (var unit in state.Units.Where(u => u.Team == myTeamId))
        {
            var enemies = state.Units.Where(u => u.Team != myTeamId);
            var closest = enemies.OrderBy(e => e.Location.Distance(unit.Location)).FirstOrDefault();
            if (closest != null)
            {
                if (closest.Location.Distance(unit.Location) <= unit.AttackDistance)
                {
                    moves.Add(new Move(MoveType.Attack, unit.Id, closest.Location));
                }
                else
                {
                    var nextStep = unit.Location.Toward(closest.Location);
                    moves.Add(new Move(MoveType.Walk, unit.Id, nextStep));
                }
            }
        }

        return Task.FromResult(moves);
    }
}
