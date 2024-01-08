using Telegram.Bot.Types;

namespace FoodPollsBot.StaticVars;

public static class StaticVariable
{
    public static string Downloadpath {  get; set; }    

    public  static string CroppedPhotos {  get; set; }

    public static List<long> AllowedUserIds = new List<long>
    {
        33780774,
        128564403
    };

    public static Poll TelegramPoll { get; set; }   

}
