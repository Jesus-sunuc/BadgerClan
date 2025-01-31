using BadgerClan.Logic;

namespace BadgerClan.Logic.Bot;

public class DefensiveBot : IBot
{
    public Task<List<Move>> PlanMovesAsync(GameState state)
    {
        var moves = new List<Move>();

        var myTeamId = state.CurrentTeamId;
        var myTeam = state.TeamList.FirstOrDefault(t => t.Id == myTeamId);

        foreach (var unit in state.Units.Where(u => u.Team == myTeamId))
        {
            var enemies = state.Units.Where(e => e.Team != myTeamId);
            var closest = enemies.OrderBy(e => e.Location.Distance(unit.Location)).FirstOrDefault();

            if (closest != null && myTeam != null)
            {
                if (myTeam.Medpacs > 0 && unit.Health < unit.MaxHealth)
                {
                    moves.Add(new Move(MoveType.Medpac, unit.Id, unit.Location));
                }
                else
                {
                    var away = unit.Location.Away(closest.Location);
                    moves.Add(new Move(MoveType.Walk, unit.Id, away));
                }
            }
        }

        return Task.FromResult(moves);
    }
}
