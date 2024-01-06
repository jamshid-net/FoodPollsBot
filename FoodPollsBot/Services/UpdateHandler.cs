using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
        
        if(update.Message.Text == "/opros")
        {
             HandlePolls(update.Message.Chat.Id, botClient).Wait();
            await RemoverFiles();
        }
       


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
      
            await botClient.DownloadFileAsync(photoFile.FilePath, fileStream, cancellationToken);

        }


    }

  
    private async Task HandlePolls(long chatId, ITelegramBotClient botClient)
    {
        var files = Directory.GetFiles(downloadpath);

        Extentions.Extentions.SetWidthHeightImage().Wait();

        var texts = await Extentions.Extentions.GetTextFromImage();

        await botClient.SendPollAsync(-1002097106323, "Obed", texts, isAnonymous: false,  allowsMultipleAnswers: true);

    }


    


    private async Task RemoverFiles()
    {
        var croppedFiles = Directory.GetFiles("../CroppedPhotos/").ToList();
        var photoFiles = Directory.GetFiles(downloadpath).ToList();

        croppedFiles.ForEach(photo =>
        {
            System.IO.File.Delete(photo);
        });
        photoFiles.ForEach(photo =>
        {
            System.IO.File.Delete(photo);
        });

    }

    private Task HandleUnknownMessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received {updateType} update.", update.Type);

        return Task.CompletedTask;
    }
   
}
