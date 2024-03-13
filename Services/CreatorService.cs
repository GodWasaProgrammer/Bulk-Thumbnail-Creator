namespace BulkThumbnailCreator.Services
{
    public class CreatorService
    {
        public CreatorService(ILogService _logger, JobService JS, Settings settings)
        {
            // if we are not in a state where we have a job, we should clear the output directories
            // this should only be called when the app is started
            // or if the joblist has been cleared
            if (UserStateService.UserJobs.Count == 0)
            {
                ClearBaseOutPutDirectories(settings);
            }
            PicDataServiceList = [];
            OutputFileServiceList = [];
            settings.LogService = _logger;
            settings.JobService = JS;
        }

        public event EventHandler<bool> LoadingStateChanged;

        private bool isLoading = false;

        public bool IsLoading
        {
            get => isLoading;
            private set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    LoadingStateChanged?.Invoke(this, isLoading);
                }
            }
        }

        // should also perhaps be derived from a current job?
        public List<PictureData> PicDataServiceList { get; set; } = [];

        // not implemented yet but should store all the produced files location of output
        public List<string> OutputFileServiceList { get; set; } = [];

        private Job CurrentJob;

        public async Task CreateInitialPictureArrayAsync(string url, List<string> ListOfTextToPrint, Settings settings, Job PassCrntJob)
        {
            IsLoading = true;

            CurrentJob = PassCrntJob;

            CurrentJob.TextToPrint = ListOfTextToPrint;

            ProductionType ProdType = ProductionType.FrontPagePictureLineUp;
            CurrentJob.TextToPrint = ListOfTextToPrint;

            if (Settings.Mocking == true && Settings.MakeMocking)
            {
                PicDataServiceList = await Creator.MockProcess(ProdType, url, ListOfTextToPrint, settings);
            }
            else
            {
                PicDataServiceList = await Creator.Process(ProdType, url, ListOfTextToPrint, settings);
            }

            if (settings.PathToVideo != null)
                CurrentJob.VideoPath = settings.PathToVideo;

            if (settings.PathToVideo != null)
                CurrentJob.VideoName = Path.GetFileNameWithoutExtension(settings.PathToVideo);

            // sets the list on the job object
            CurrentJob.PictureDatas = PicDataServiceList;

            CurrentJob.State = States.FrontPagePictureLineUp;

            UserStateService.UpdateJob(CurrentJob);

            IsLoading = false;
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData PicToVarietize, Settings settings, Job PassCrntJob)
        {
            PassCrntJob.Settings = settings;
            CurrentJob = PassCrntJob;

            IsLoading = true;

            CurrentJob.State = States.varietyList;

            List<string> ImageUrls = [];
            string url = string.Empty;

            ProductionType ProdType = ProductionType.VarietyList;
            if (Settings.Mocking == true && Settings.MakeMocking)
            {
                PicDataServiceList = await Creator.MockProcess(ProdType, url, CurrentJob.TextToPrint, settings, PicToVarietize);
            }
            else
            {
                PicDataServiceList = await Creator.Process(ProdType, url, CurrentJob.TextToPrint, CurrentJob.Settings, PicToVarietize);
            }

            // sets the list on the job object
            CurrentJob.PictureDatas = PicDataServiceList;

            string liftMockFolder = "";
            if(Settings.Mocking == true)
            {
                string[] subdirs = Directory.GetDirectories(settings.TextAddedDir);
                foreach(var subdir in subdirs)
                {
                    string lastFolderName = Path.GetFileName(subdir.TrimEnd(Path.DirectorySeparatorChar));
                    liftMockFolder = lastFolderName;
                }
                liftMockFolder = liftMockFolder.Replace("varietyof", "");

                string liftFileName;
                liftFileName = CurrentJob.PictureDatas[0].FileName;
                string path = Path.GetDirectoryName(liftFileName);

                liftMockFolder = Path.Combine(path,liftMockFolder);
                int index = liftMockFolder.IndexOf("\\");
                liftMockFolder = liftMockFolder.Remove(index, 1).Insert(index, "/");
               
                PicToVarietize = CurrentJob.PictureDatas.Find(x => x.FileName == liftMockFolder);
            }

            string parentfilename = Path.GetFileName(PicToVarietize.FileName);
            string varietyof = "varietyof";
            string ConcatenatedString = $"{PassCrntJob.Settings.TextAddedDir}/{varietyof}{parentfilename}";
            string[] ArrayOfFilePaths = Directory.GetFiles(ConcatenatedString, "*.png");

            foreach (string filepath in ArrayOfFilePaths)
            {
                string imageurl = $"/{filepath}"; // convert to URL
                ImageUrls.Add(imageurl);
            }

            // the list of urls to be displayed in variety display

            CurrentJob.VarietyUrls = ImageUrls;

            UserStateService.UpdateJob(CurrentJob);

            IsLoading = false;

            return ImageUrls;
        }

        public async Task<PictureData> CreateCustomPicDataObject(PictureData PicToCustomize, Settings settings, Job PassCrntJob)
        {
            ArgumentNullException.ThrowIfNull(settings);

            CurrentJob = PassCrntJob;

            isLoading = true;

            CurrentJob.State = States.CustomPicture;
            string url = string.Empty;
            PicDataServiceList = await Creator.Process(ProductionType.CustomPicture, url, CurrentJob.TextToPrint, CurrentJob.Settings, PicToCustomize);
            CurrentJob.PictureDatas = PicDataServiceList;

            UserStateService.UpdateJob(CurrentJob);

            isLoading = false;

            return PicToCustomize;
        }

        public Task<PictureData> SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            PictureData PicData = new();

            if (Settings.Mocking == true && Settings.MakeMocking)
            {
                string DirToMockPicture = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", $"TextAdded");

                DirectoryInfo di = new(DirToMockPicture);

                DirectoryInfo[] di2 = di.GetDirectories();

                string MockCorrelation = "";

                foreach (DirectoryInfo directory in di2)
                {
                    MockCorrelation = Path.GetFileName(directory.FullName);
                }

                foreach (var item in PicDataServiceList)
                {
                    string NumberOfPicture = Path.GetFileNameWithoutExtension(item.FileName);

                    string NumberOfPicture2 = Path.GetFileNameWithoutExtension(MockCorrelation);

                    string varof = "varietyof ";

                    NumberOfPicture2 = NumberOfPicture2.Remove(0, varof.Length);

                    if (NumberOfPicture == NumberOfPicture2)
                    {
                        PicData = new PictureData(item);
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in PicDataServiceList)
                {
                    var url = Path.GetFileNameWithoutExtension(imageUrl);
                    var outpath = Path.GetFileNameWithoutExtension(item.OutPath);

                    if (Path.GetFileNameWithoutExtension(item.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                    {
                        PicData = new PictureData(item);
                        break;
                    }
                }
            }

            return Task.FromResult(PicData);
        }

        public PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl)
        {
            PictureData PicData = new();

            if (Settings.Mocking == true && Settings.MakeMocking)
            {
                // nonsense to just pick the first one
                foreach (var item in PicDataServiceList)
                {
                    foreach (PictureData variety in item.Varieties)
                    {
                        PicData = new PictureData(variety);
                        break;
                    }
                    if (PicData != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in PicDataServiceList)
                {
                    foreach (PictureData variety in item.Varieties)
                    {
                        if (Path.GetFileNameWithoutExtension(variety.OutPath) == Path.GetFileNameWithoutExtension(imageUrl))
                        {
                            PicData = new PictureData(variety);
                            break;
                        }
                    }
                }
            }
            return PicData;
        }

        public static void ClearBaseOutPutDirectories(Settings settings)
        {
            DirectoryInfo di = new(settings.TextAddedDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // TODO : for whatever reason these resources are not free for deletion
            DirectoryInfo di2 = new(settings.OutputDir);

            foreach (FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}