using FoodPollsBot.Interfaces;
using System.Drawing;
using System.Text.RegularExpressions;
using Tesseract;
using static FoodPollsBot.StaticVars.StaticVariable;

namespace FoodPollsBot.Services;

public class ImageOperationsService(IConfiguration configuration) : IImageOperationService
{
    public  Task<List<string>> GetTextFromImage()
    {
        var croppedPhotosFile = Directory.GetFiles(CroppedPhotos);
        var textsHash = new HashSet<string>();
        foreach (var file in croppedPhotosFile)
        {
            using (var engine = new TesseractEngine("../tessdata", "rus", EngineMode.Default))
            {

                using (var img = Pix.LoadFromFile(file))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            text = "РАКУШКА";
                        }
                        string cleanedText = Regex.Replace(text, @"[\s_]+", " ");

                        textsHash.Add(cleanedText);
                    }
                }

            }

        }

        return Task.FromResult(textsHash.ToList());
    }

    public async Task SetWidthHeightImage()
    {
        var counter = 0;
        var tasks = new List<Task>();

        var files = Directory.GetFiles(Downloadpath);
        foreach (var file in files)
        {

            var t = Task.Run(() =>
            {
                Bitmap originalImage = new Bitmap(file);

                // Specify the crop area (for example, cropping 20% from the top, left, right, and bottom)
                //int cropPercentage = 20;
                //int cropWidth = (int)(originalImage.Width * (1 - cropPercentage / 100.0));
                //int cropHeight = (int)(originalImage.Height * (1 - cropPercentage / 100.0));
                Rectangle cropArea = new Rectangle(100, 150, 470, 230);


                Bitmap croppedImage = new Bitmap(470, 230);
                using (Graphics g = Graphics.FromImage(croppedImage))
                {
                    g.DrawImage(originalImage, new Rectangle(0, 0, 500, 250), cropArea, GraphicsUnit.Pixel);
                }

                croppedImage.Save(CroppedPhotos+$"file{counter++}.jpg");

                originalImage.Dispose();
                croppedImage.Dispose();
            });
            tasks.Add(t);
        }

        await Task.WhenAll(tasks).ConfigureAwait(true);
    }

    public Task RemoverFiles()
    {
        var croppedFiles = Directory.GetFiles(CroppedPhotos).ToList();
        var photoFiles = Directory.GetFiles(Downloadpath).ToList();

        croppedFiles.ForEach(photo =>
        {
            System.IO.File.Delete(photo);
        });
        photoFiles.ForEach(photo =>
        {
            System.IO.File.Delete(photo);
        });

        return Task.CompletedTask;
    }
}
