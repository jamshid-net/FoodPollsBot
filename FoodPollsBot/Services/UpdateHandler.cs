using FoodPollsBot.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static FoodPollsBot.StaticVars.StaticVariable;
namespace FoodPollsBot.Services;

public class UpdateHandler(ILogger<UpdateHandler> _logger, 
                            IConfiguration configuration,
                            IImageOperationService imageService,
                            GetCommandsService commands) : IUpdateHandler
{
    private long _groupChatId => configuration.GetValue<long>("GroupChatId");
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception.Message, "Error while polling telegram bot");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
       
        if (!AllowedUserIds.Any(id => id == update.Message.From.Id))
            return;

        if (update.Message is null) return;
        var handleTask = update.Type switch
        {

            UpdateType.Message => HandleMessageUpdateAsync(botClient, update, cancellationToken),

            _ => HandleUnknownMessageUpdateAsync(botClient, update, cancellationToken)
        };
        
        try
        {
            await handleTask;
        }
        catch (Exception ex)
        {

            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }


    private async Task HandleMessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        
        if(!string.IsNullOrEmpty(update.Message.Text) &&  update.Message.Text.StartsWith("/"))
        {
            await HandleCommands(update, cancellationToken);
        }

        var photos = update.Message.Photo;
      
        if (photos.Length > 0)
        {
             
            var photo = photos.Last();

            var photoFile =  await botClient.GetFileAsync(photo.FileId, cancellationToken);

            _logger.LogInformation(photoFile.FilePath);

            var fileName = Path.GetFileName(photoFile.FilePath);  

            await using Stream fileStream = System.IO.File.Create(Downloadpath + fileName);
      
            await botClient.DownloadFileAsync(photoFile.FilePath, fileStream, cancellationToken);

        }


    }

    private async Task HandleCommands(Update update, CancellationToken cancellationToken)
    {
        foreach (var command in commands.GetCommands())
        {
            if(command.Name == update.Message.Text)
            {
              await command.ExecuteAsync(_groupChatId, update, cancellationToken);
            }

        }
    }

    private Task HandleUnknownMessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received {updateType} update.", update.Type);

        return Task.CompletedTask;
    }
   
}
