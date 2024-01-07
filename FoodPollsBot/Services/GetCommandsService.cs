using FoodPollsBot.Interfaces;

namespace FoodPollsBot.Services;

public class GetCommandsService
    (
    [FromKeyedServices("opros")] ITelegramCommand oprosCommand,
     [FromKeyedServices("getmoney")] ITelegramCommand getmoneyCommand,
     [FromKeyedServices("getPollResult")] ITelegramCommand getPollResult
    )
{
    List<ITelegramCommand> commands;
    public List<ITelegramCommand> GetCommands()
    {
        commands = new List<ITelegramCommand>()
        {
            oprosCommand,
            getmoneyCommand,
            getPollResult
        };

        return commands;
    }
}
