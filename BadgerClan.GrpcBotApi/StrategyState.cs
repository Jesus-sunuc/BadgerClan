using BadgerClan.Logic.Bot;

namespace BadgerClan.GrpcBotApi;

public static class StrategyState
{
    private static IBot _currentBot = new NothingBot();

    public static IBot CurrentBot
    {
        get => _currentBot;
        set => _currentBot = value;
    }
}