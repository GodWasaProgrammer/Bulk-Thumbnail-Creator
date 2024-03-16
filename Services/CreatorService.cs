// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Ignore Spelling: Pic

namespace BulkThumbnailCreator.Services
{
    public class CreatorService
    {
        public CreatorService(ILogService logger, JobService jobService, Settings settings)
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
            settings.LogService = logger;
            settings.JobService = jobService;
        }

        public event EventHandler<bool> LoadingStateChanged;

        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    LoadingStateChanged?.Invoke(this, _isLoading);
                }
            }
        }

        // should also perhaps be derived from a current job?
        public List<PictureData> PicDataServiceList { get; set; }

        // not implemented yet but should store all the produced files location of output
        public List<string> OutputFileServiceList { get; set; }

        private Job _currentJob;

        public async Task CreateInitialPictureArrayAsync(string url, List<string> listOfTextToPrint, Settings settings, Job job)
        {
            IsLoading = true;

            _currentJob = job;

            _currentJob.TextToPrint = listOfTextToPrint;

            var prodType = ProductionType.FrontPagePictureLineUp;
            _currentJob.TextToPrint = listOfTextToPrint;

            if (Settings.Mocking && Settings.MakeMocking)
            {
                PicDataServiceList = await Creator.MockProcess(prodType, url, listOfTextToPrint, settings);
            }
            else
            {
                PicDataServiceList = await Creator.Process(prodType, url, listOfTextToPrint, settings);
            }

            if (settings.PathToVideo != null)
                _currentJob.VideoPath = settings.PathToVideo;

            if (settings.PathToVideo != null)
                _currentJob.VideoName = Path.GetFileNameWithoutExtension(settings.PathToVideo);

            // sets the list on the job object
            _currentJob.PictureData = PicDataServiceList;

            _currentJob.State = States.FrontPagePictureLineUp;

            UserStateService.UpdateJob(_currentJob);

            IsLoading = false;
        }

        public async Task<List<string>> CreatePictureDataVariety(PictureData pictureData, Settings settings, Job job)
        {
            job.Settings = settings;
            _currentJob = job;

            IsLoading = true;

            _currentJob.State = States.varietyList;

            List<string> imageUrls = [];
            var url = string.Empty;

            var prodType = ProductionType.VarietyList;
            if (Settings.Mocking && Settings.MakeMocking)
            {
                PicDataServiceList = await Creator.MockProcess(prodType, url, _currentJob.TextToPrint, settings, pictureData);
            }
            else
            {
                PicDataServiceList = await Creator.Process(prodType, url, _currentJob.TextToPrint, _currentJob.Settings, pictureData);
            }

            // sets the list on the job object
            _currentJob.PictureData = PicDataServiceList;

            var liftMockFolder = "";
            if (Settings.Mocking)
            {
                var subdirs = Directory.GetDirectories(settings.TextAddedDir);
                foreach (var subdir in subdirs)
                {
                    var lastFolderName = Path.GetFileName(subdir.TrimEnd(Path.DirectorySeparatorChar));
                    liftMockFolder = lastFolderName;
                }
                liftMockFolder = liftMockFolder.Replace("varietyof", "");

                string liftFileName;
                liftFileName = _currentJob.PictureData[0].FileName;
                var path = Path.GetDirectoryName(liftFileName);

                liftMockFolder = Path.Combine(path, liftMockFolder);
                var index = liftMockFolder.IndexOf("\\");
                liftMockFolder = liftMockFolder.Remove(index, 1).Insert(index, "/");

                pictureData = _currentJob.PictureData.Find(x => x.FileName == liftMockFolder);
            }

            var parentfilename = Path.GetFileName(pictureData.FileName);
            var varietyof = "varietyof";
            var concatenatedString = $"{job.Settings.TextAddedDir}/{varietyof}{parentfilename}";
            var arrayOfFilePaths = Directory.GetFiles(concatenatedString, "*.png");

            foreach (var filepath in arrayOfFilePaths)
            {
                var imageurl = $"/{filepath}"; // convert to URL
                imageUrls.Add(imageurl);
            }

            // the list of urls to be displayed in variety display

            _currentJob.VarietyUrls = imageUrls;

            UserStateService.UpdateJob(_currentJob);

            IsLoading = false;

            return imageUrls;
        }

        public async Task<PictureData> CreateCustomPicDataObject(PictureData pictureData, Settings settings, Job job)
        {
            ArgumentNullException.ThrowIfNull(settings);

            _currentJob = job;

            _isLoading = true;

            _currentJob.State = States.CustomPicture;
            var url = string.Empty;
            PicDataServiceList = await Creator.Process(ProductionType.CustomPicture, url, _currentJob.TextToPrint, _currentJob.Settings, pictureData);
            _currentJob.PictureData = PicDataServiceList;

            UserStateService.UpdateJob(_currentJob);

            _isLoading = false;

            return pictureData;
        }

        public Task<PictureData> SetPictureDataImageDisplayCorrelation(string imageUrl)
        {
            PictureData picData = new();

            if (Settings.Mocking && Settings.MakeMocking)
            {
                var dirToMockPicture = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", $"TextAdded");

                DirectoryInfo di = new(dirToMockPicture);

                var di2 = di.GetDirectories();

                var mockCorrelation = "";

                foreach (var directory in di2)
                {
                    mockCorrelation = Path.GetFileName(directory.FullName);
                }

                foreach (var item in PicDataServiceList)
                {
                    var numberOfPicture = Path.GetFileNameWithoutExtension(item.FileName);

                    var numberOfPicture2 = Path.GetFileNameWithoutExtension(mockCorrelation);

                    var varof = "varietyof ";

                    numberOfPicture2 = numberOfPicture2.Remove(0, varof.Length);

                    if (numberOfPicture == numberOfPicture2)
                    {
                        picData = new PictureData(item);
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in PicDataServiceList)
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

        public PictureData SetPictureDataImageDisplayCorrelationForVarietyList(string imageUrl)
        {
            PictureData picData = new();

            if (Settings.Mocking == true && Settings.MakeMocking)
            {
                // nonsense to just pick the first one
                foreach (var item in PicDataServiceList)
                {
                    foreach (var variety in item.Varieties)
                    {
                        picData = new PictureData(variety);
                        break;
                    }
                    if (picData != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in PicDataServiceList)
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

        public static void ClearBaseOutPutDirectories(Settings settings)
        {
            DirectoryInfo di = new(settings.TextAddedDir);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            DirectoryInfo di2 = new(settings.OutputDir);

            foreach (var file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di2.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
