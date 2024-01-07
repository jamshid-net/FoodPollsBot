using Telegram.Bot.Types;
using Telegram.Bot;

namespace FoodPollsBot.Interfaces;

public interface ITelegramCommand
{
    public ITelegramBotClient Client { get; }

    public string Name { get; }

    public Task ExecuteAsync(long gropChatId, Update update, CancellationToken token = default);


}
