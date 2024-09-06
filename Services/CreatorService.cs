namespace BulkThumbnailCreator.Services;

public class CreatorService
{
    public CreatorService(Creator creator)
    {
        _creator = creator;
    }

    private Creator _creator;

    public static Task<PictureData> SetPictureDataImageDisplayCorrelation(string imageUrl, Job job)
    {
        PictureData picData = new();

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            var dirToMockPicture = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "TextAdded");

            DirectoryInfo di = new(dirToMockPicture);

            var di2 = di.GetDirectories();

            var mockCorrelation = "";

            foreach (var directory in di2)
            {
                mockCorrelation = Path.GetFileName(directory.FullName);
            }

            foreach (var item in job.PictureData)
            {
                var numberOfPicture = Path.GetFileNameWithoutExtension(item.FileName);

                var numberOfPicture2 = Path.GetFileNameWithoutExtension(mockCorrelation);

                const string Varof = "varietyof ";

                numberOfPicture2 = numberOfPicture2.Remove(0, Varof.Length);

                if (numberOfPicture == numberOfPicture2)
                {
                    picData = new PictureData(item);
                    break;
                }
            }
        }
        else
        {
            foreach (var item in job.PictureData)
            {
                if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                {
                    picData = new PictureData(item);
                    break;
                }
            }
        }

        return Task.FromResult(picData);
    }

    public static PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl, Job job)
    {
        PictureData picData = new();

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            // nonsense to just pick the first one
            var jobdata = job.PictureData[0];
            var jobvarietyData = jobdata.Varieties[0];
            picData = jobvarietyData;
        }
        else
        {
            foreach (var item in job.PictureData)
            {
                foreach (var variety in item.Varieties)
                {
                    if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        picData = new PictureData(variety);
                        break;
                    }
                }
            }
        }
        return picData;
    }

    public async Task<string> FetchVideo(string urlToVideo, Settings settings)
    {
        var pathToVideo = await _creator.FetchVideo(urlToVideo, settings);
        return pathToVideo;
    }
}
