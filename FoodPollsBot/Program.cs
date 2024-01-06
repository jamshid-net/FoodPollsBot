using FoodPollsBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddSingleton<ITelegramBotClient>(
             new TelegramBotClient(builder.Configuration?.GetValue<string>("telegramBot")));

builder.Services.AddHostedService<BotBackgroundService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
