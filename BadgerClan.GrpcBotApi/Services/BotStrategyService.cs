using BadgerClan.GrpcBotApi;
using BadgerClan.Logic.Bot;
using Grpc.Core;

namespace BadgerClan.GrpcBotApi.Services;

public class BotStrategyService : BotStrategy.BotStrategyBase
{
    private static IBot _currentBot = new NothingBot();

    public override Task<SetStrategyReply> SetStrategy(SetStrategyRequest request, ServerCallContext context)
    {
        switch (request.StrategyName.ToLowerInvariant())
        {
            case "aggressive":
                _currentBot = new AggressiveBot();
                break;
            case "defensive":
                _currentBot = new DefensiveBot();
                break;
            case "random":
                _currentBot = new RandomBot();
                break;
            default:
                _currentBot = new NothingBot();
                break;
        }

        return Task.FromResult(new SetStrategyReply
        {
            Message = $"Strategy changed to {request.StrategyName}"
        });
    }

    public override Task<GetCurrentStrategyReply> GetCurrentStrategy(GetCurrentStrategyRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetCurrentStrategyReply
        {
            CurrentStrategy = _currentBot.GetType().Name
        });
    }
}
