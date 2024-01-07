using FoodPollsBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FoodPollsBot.Commands;

public class MoneyPollsCommand(ITelegramBotClient botClient) : ITelegramCommand
{
    public ITelegramBotClient Client => botClient;

    public string Name => "/getmoney";

    public async Task ExecuteAsync(long groupChatId,Update update, CancellationToken token = default)
    {
        List<string> questions = ["+", "Ertaga"];

        await botClient.SendPollAsync(groupChatId,"Test", questions, allowsMultipleAnswers: true, isAnonymous: false,cancellationToken:token);
    
    }
}
