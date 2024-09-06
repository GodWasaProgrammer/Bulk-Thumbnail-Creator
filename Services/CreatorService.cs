namespace BulkThumbnailCreator.Services;

public class CreatorService
{
    public CreatorService( Creator creator)
    {
        _creator = creator;
    }

    private Creator _creator;
    public async Task<List<string>> CreatePictureDataVariety(PictureData pictureData, Job job)
    {
        List<string> imageUrls = [];
        var url = string.Empty;

        if (job.Settings.Mocking && job.Settings.MakeMocking)
        {
            job.PictureData = await _creator.MockProcess(ProductionType.VarietyList, url, job.TextToPrint, job, pictureData);
            job.State = States.varietyList;
        }
        else
        {
            job.State = States.Loading;
            // job.PictureData = await _creator.VarietyLineup(job, pictureData);
            job.State = States.varietyList;
        }

        var liftMockFolder = "";
        if (job.Settings.Mocking)
        {
            var subdirs = Directory.GetDirectories(job.Settings.TextAddedDir);
            foreach (var subdir in subdirs)
            {
                var lastFolderName = Path.GetFileName(subdir.TrimEnd(Path.DirectorySeparatorChar));
                liftMockFolder = lastFolderName;
            }
            liftMockFolder = liftMockFolder.Replace("varietyof", "");
            var path = Path.GetDirectoryName(job.PictureData[0].FileName);

            liftMockFolder = Path.Combine(path, liftMockFolder);
            var index = liftMockFolder.IndexOf("\\");
            liftMockFolder = liftMockFolder.Remove(index, 1).Insert(index, "/");

            pictureData = job.PictureData.Find(x => x.FileName == liftMockFolder);
        }

        var parentfilename = Path.GetFileName(pictureData.FileName);
        var concatenatedString = $"{job.Settings.TextAddedDir}/varietyof{parentfilename}";
        var arrayOfFilePaths = Directory.GetFiles(concatenatedString, "*.png");

        foreach (var filepath in arrayOfFilePaths)
        {
            var imageurl = $"/{filepath}"; // convert to URL
            imageUrls.Add(imageurl);
        }

        // the list of urls to be displayed in variety display
        job.VarietyUrls = imageUrls;

        return imageUrls;
    }

    public async Task CreateRandomVariety(PictureData pictureData, Job job)
    {
        await _creator.Random(job, pictureData);
    }

    public async Task CreateFontVariety(PictureData pictureData, Job job)
    {

        await _creator.FontVariety(job, pictureData);

    }
    public async Task CreateFXVariety(PictureData pictureData, Job job)
    {
        await _creator.SpecialEffectsVariety(job, pictureData);
    }

    public async Task CreateBoxVariety(PictureData pictureData, Job job)
    {
        await _creator.BoxVariety(job, pictureData);
    }

    public async Task CreateColorVariety(PictureData pictureData, Job job)
    {
        await _creator.ColorVariety(job, pictureData);
    }

    public async Task<PictureData> CreateCustomPicDataObject(PictureData pictureData, Job job)
    {
        job.State = States.CustomPicture;
        job.PictureData = await _creator.CustomPicture(job, pictureData);

        return pictureData;
    }

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
