using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace FoodPollsBot.Controllers;
[ApiController]
public class BotStatusController(ITelegramBotClient botClient) : ControllerBase
{

    [HttpGet]
    [Route("status")]
    public async Task<string> GetBotStatus(CancellationToken token)
    {
        var me = await botClient.GetMeAsync(token);

        return me.Username ?? "bot not found";
    }
}
