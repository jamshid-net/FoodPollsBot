namespace FoodPollsBot.Interfaces;

public interface IImageOperationService
{
    Task SetWidthHeightImage();

    Task<List<string>> GetTextFromImage();
    Task RemoverFiles();
}
