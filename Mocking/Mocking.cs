namespace BulkThumbnailCreator;

internal class Mocking
{
    public static int BTCRunCount = 0;

    internal static void SerializePicData(Settings settings)
    {
        // XmlSerializer Serializer = new(typeof(List<PictureData>));

        //// Open the file for writing or create a new one if it doesn't exist
        //using (StreamWriter streamWriter = new("mockFP.xml"))
        //{
        //    // Serialize the entire list at once
        //    Serializer.Serialize(streamWriter, settings.PictureDatas);
        //}
        settings.LogService.LogInformation("Settings.PictureDatas Serialized to mockFP.xml");

        string mockDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "mockFP.xml");
        File.Copy("mockFP.xml", mockDir, true);

        // clean up the text added dir of mockfolder
        string mockdir3 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

        DirectoryInfo di3 = new(mockdir3);

        foreach (FileInfo file in di3.GetFiles())
        {
            file.Delete();
        }

        // Copy the Text Added Directory to Mock Folder
        string mockdir2 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "text added");

        string[] Mockfiles = Directory.GetFiles(settings.TextAddedDir);

        foreach (string file in Mockfiles)
        {
            File.Copy(file, $"{mockdir2}/{Path.GetFileName(file)}", true);
        }
    }

    internal static async Task SetupVarietyDisplay(Settings settings)
    {
        // copy pictures to text added dir / var dir                    // copy pictures to text added dir / var dir
        string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

        string[] Directories = Directory.GetDirectories(sourceDirectory);

        foreach (string dir in Directories)
        {
            // this should only have one directory to copy
            // since we are mocking the variety list

            string[] files = Directory.GetFiles(dir);

            string dirname = Path.GetFileName(dir);

            Directory.CreateDirectory(Path.Combine(settings.TextAddedDir, dirname));

            foreach (string file in files)
            {
                string outPath = Path.Combine(settings.TextAddedDir, dirname);
                outPath = Path.Combine(outPath, Path.GetFileName(file));

                await Task.Run(() => File.Copy(file, outPath));
                settings.LogService.LogInformation($"Copied {file} to {outPath}");
            }
        }
    }

    internal static async Task SetupFrontPagePictureLineUp(Settings settings)
    {
        // pretend to make line up
        string srcMockFolder = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "output");
        string[] outputDirList = Directory.GetFiles(srcMockFolder);

        // copy outputDir to mockOutPutDir ??
        foreach (string outputFile in outputDirList)
        {
            string completePath = Path.Combine(settings.OutputDir, Path.GetFileName(outputFile));
            await Task.Run(() => File.Copy(outputFile, completePath));
        }

        // copy pictures to text added dir
        string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

        string[] files = Directory.GetFiles(sourceDirectory);

        foreach (string file in files)
        {
            await Task.Run(() => File.Copy(file, $"{settings.TextAddedDir}/{Path.GetFileName(file)}"));
        }

        srcMockFolder = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp");

        try
        {
            srcMockFolder = Path.Combine(srcMockFolder, "mockFP.xml");
        }
        catch (Exception)
        {
            settings.LogService.LogError("Could not find mockFP.xml");
            throw;
        }

        List<PictureData> deserializedList;

        // Create a XmlSerializer for the list of PictureData
        XmlSerializer xmlSerializer = new(typeof(List<PictureData>));

        using (StreamReader streamReader = new(srcMockFolder))
        {
            deserializedList = (List<PictureData>)xmlSerializer.Deserialize(streamReader);
        }
        // add the deserialized list to the PictureDatas list
        // this completes the mocking of the process
        settings.PictureDatas = deserializedList;
    }

    internal static void OutPutDirMockCopy(Settings settings)
    {
        // first we will clear the directory of any files
        string mockOutPutDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "output");
        string[] mockOutPutDirFiles = Directory.GetFiles(mockOutPutDir);

        foreach (var MockOutPutFile in mockOutPutDirFiles)
        {
            File.Delete(MockOutPutFile);
        }

        // Mock copying of outputDir to mockOutPutDir
        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);

        if (Directory.Exists(mockOutPutDir))
        {
            foreach (var file in settings.Files)
            {
                string filename = Path.GetFileName(file);
                string writePath = Path.Combine(mockOutPutDir, filename);
                File.Copy(file, writePath);
            }
        }
    }

    internal static async Task VarietiesList(Settings settings)
    {
        DirectoryInfo di = new(settings.TextAddedDir);

        DirectoryInfo[] di2 = di.GetDirectories();

        foreach (DirectoryInfo dir in di2)
        {
            string[] files = Directory.GetFiles(dir.FullName);

            string TargetDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "text added");

            // add the variety directory to the target directory

            Directory.CreateDirectory($"{TargetDirectory}/{dir.Name}");

            TargetDirectory = Path.Combine(TargetDirectory, dir.Name);

            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);

                if (!File.Exists($"{TargetDirectory}/{filename}"))
                {
                    await Task.Run(() => File.Copy(file, $"{TargetDirectory}/{filename}"));
                }
            }
        }
    }
}