
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Tesseract;

namespace FoodPollsBot.Services;

public class UpdateHandler(ILogger<UpdateHandler> _logger, IConfiguration configuration) : IUpdateHandler
{

    private string downloadpath => configuration["downloadPath"] ?? "../";

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception.Message, "Error while polling telegram bot");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
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


        var photos = update.Message.Photo;

        if (photos.Length > 0)
        {
             
            var photo = photos.Last();

            var photoFile =  await botClient.GetFileAsync(photo.FileId, cancellationToken);

            _logger.LogInformation(photoFile.FilePath);

            

            var fileName = Path.GetFileName(photoFile.FilePath);  

           

            await using Stream fileStream = System.IO.File.Create(downloadpath + fileName);

            var downloadTask = botClient.DownloadFileAsync(photoFile.FilePath, fileStream);

            await downloadTask.ContinueWith((t) =>
            {
               var filesNow =  Directory.GetFiles(downloadpath);

                
                using (var engine = new TesseractEngine("../tessdata", "rus", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(filesNow[0]))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            _logger.LogInformation(text);   
                        }
                    }
                }


            });

            await downloadTask;
        }

    }

  

    private Task HandleUnknownMessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received {updateType} update.", update.Type);

        return Task.CompletedTask;
    }
   
}
