namespace BulkThumbnailCreator.Services;

public class CreatorService
{
    public CreatorService(Creator creator)
    {
        _creator = creator;
    }

    private Creator _creator;

    public async Task<string> FetchVideo(string urlToVideo, Settings settings)
    {
        var pathToVideo = await _creator.FetchVideo(urlToVideo, settings);
        return pathToVideo;
    }
}
