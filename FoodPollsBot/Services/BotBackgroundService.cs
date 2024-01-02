
using Telegram.Bot.Polling;
using Telegram.Bot;

namespace FoodPollsBot.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<BotBackgroundService> _logger;


    public BotBackgroundService(ITelegramBotClient botClient, IUpdateHandler updateHandler, ILogger<BotBackgroundService> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await _botClient.GetMeAsync(stoppingToken);

        _logger.LogInformation("Bot started:{username}", me.Username);

        _botClient.StartReceiving(
            updateHandler: _updateHandler,
            receiverOptions: default,
            cancellationToken: stoppingToken);
    }
}
