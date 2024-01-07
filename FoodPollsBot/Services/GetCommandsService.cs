using FoodPollsBot.Interfaces;

namespace FoodPollsBot.Services;

public class GetCommandsService
    ([FromKeyedServices("opros")] ITelegramCommand oprosCommand,
     [FromKeyedServices("getmoney")] ITelegramCommand getmoneyCommand)
{
    List<ITelegramCommand> commands;
    public List<ITelegramCommand> GetCommands()
    {
        commands = new List<ITelegramCommand>()
        {
            oprosCommand,
            getmoneyCommand
        };

        return commands;
    }
}
