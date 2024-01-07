using ConsoleTables;
using FoodPollsBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using static FoodPollsBot.StaticVars.StaticVariable;
namespace FoodPollsBot.Commands;

public class GetPollResultCommand(ITelegramBotClient botClient) : ITelegramCommand
{
    public ITelegramBotClient Client => botClient;

    public string Name => "/result";

    public async Task ExecuteAsync(long gropChatId, Update update, CancellationToken token = default)
    {
        //if (LastPollId is null)
        //    return;


       

          

        var table = new ConsoleTable("Food", "Count");

        foreach (var poll in update.Poll.Options)
        {
            table.AddRow(poll.Text, poll.VoterCount);
        }

        var result = table.ToStringAlternative();

        await botClient.SendTextMessageAsync(gropChatId, result);
       
    }
}
