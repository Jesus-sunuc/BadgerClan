using BadgerClan.GrpcBotApi;
using BadgerClan.Logic.Bot;
using Grpc.Core;

namespace BadgerClan.GrpcBotApi.Services;

public class BotStrategyService : BotStrategy.BotStrategyBase
{
    public override Task<SetStrategyReply> SetStrategy(SetStrategyRequest request, ServerCallContext context)
    {
        switch (request.StrategyName.ToLowerInvariant())
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

        return Task.FromResult(new SetStrategyReply
        {
            Message = $"Strategy changed to {request.StrategyName}"
        });
    }

    public override Task<GetCurrentStrategyReply> GetCurrentStrategy(GetCurrentStrategyRequest request, ServerCallContext context)
    {
        var current = StrategyState.CurrentBot.GetType().Name;
        return Task.FromResult(new GetCurrentStrategyReply
        {
            CurrentStrategy = current
        });
    }
}