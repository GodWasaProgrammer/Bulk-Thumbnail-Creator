namespace BulkThumbnailCreator.Services;

public interface IProduction
{
    void CreateDirectories(Settings settings);
    Task VerifyDirectoryAndExeIntegrity(Settings settings);
    Task YouTubeDL(Job job);
    Task ProduceTextPictures(PictureData picData, Settings settings);
}
