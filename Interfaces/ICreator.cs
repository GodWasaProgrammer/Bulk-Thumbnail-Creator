namespace BulkThumbnailCreator;

public interface ICreator
{
    event EventHandler<bool> LoadingStateChanged;
    bool IsLoading { get; }
    Task<string> FetchVideo(string ytlink, Settings settings);
    Task FrontPageLineup_Thumbies(Job job);
    Task FrontPageLineup(Job job);
    Task VarietyLineup(Job job, PictureData pictureData);
    Task<PictureData> CustomPicture(Job job, PictureData pictureData);
    Task Random(Job job, PictureData pictureData);
    Task FontVariety(Job job, PictureData pictureData);
    Task BoxVariety(Job job, PictureData pictureData);
    Task SpecialEffectsVariety(Job job, PictureData pictureData);
    Task ColorVariety(Job job, PictureData pictureData);
    Task<List<PictureData>> MockProcess(ProductionType productionType, string url, List<string> texts, Job job, PictureData pictureData = null);
    void ClearBaseOutPutDirectories();
}
