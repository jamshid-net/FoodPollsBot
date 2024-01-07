
using FoodPollsBot.Interfaces;
using Polly.Registry;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FoodPollsBot.Commands;

public class OprosCommand(ITelegramBotClient botClient,
                         IConfiguration configuration,
                         IImageOperationService imageService,
                         ResiliencePipelineProvider<string> pollyPipeline) : ITelegramCommand
{


    public ITelegramBotClient Client => botClient;

    public string Name => "/opros";

    public async Task ExecuteAsync(long groupChatid, Update update, CancellationToken token)
    {
        Task task = null;

        var IsCompleted = pollyPipeline
             .GetPipeline("opros-pipeline")
             .ExecuteAsync(async token2 =>
             {
                 task = HandlePolls(groupChatid, token2);
             });


        await task.ContinueWith(t=> imageService.RemoverFiles());
        

    }

    private async Task HandlePolls(long groupChatId, CancellationToken token)
    {

        imageService.SetWidthHeightImage().Wait();

        var texts = await imageService.GetTextFromImage();

        await botClient.SendPollAsync(groupChatId, "Obed", texts, isAnonymous: false, allowsMultipleAnswers: true, cancellationToken: token);

    }
}
