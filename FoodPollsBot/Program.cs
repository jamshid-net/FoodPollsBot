using FoodPollsBot.Commands;
using FoodPollsBot.Interfaces;
using FoodPollsBot.Services;
using FoodPollsBot.StaticVars;
using Polly;
using Polly.Retry;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);



LoadStaticProperties(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddResiliencePipeline("opros-pipeline", cfg =>
{
    cfg.AddRetry(new RetryStrategyOptions())
        .AddTimeout(TimeSpan.FromSeconds(7));
});

builder.Services.AddSingleton<IImageOperationService,ImageOperationsService>();
builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();
builder.Services.AddSingleton<ITelegramBotClient>(
             new TelegramBotClient(builder.Configuration?.GetValue<string>("telegramBot")));

builder.Services.AddKeyedSingleton<ITelegramCommand, OprosCommand>("opros");
builder.Services.AddKeyedSingleton<ITelegramCommand, MoneyPollsCommand>("getmoney");



builder.Services.AddSingleton<GetCommandsService>();


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


void LoadStaticProperties(IConfiguration configuration)
{
    StaticVariable.CroppedPhotos = configuration["FilesPath:photosCutPath"] ?? string.Empty;
    StaticVariable.Downloadpath = configuration["FilesPath:telegramDownloadPath"] ?? string.Empty;
}