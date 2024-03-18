namespace BulkThumbnailCreator;

internal static class Mocking
{
    // this class is displayed in the order it is called for easy understanding of its structure
    public static int BTCRunCount = 0;

    internal static void SerializePicData(Settings settings)
    {
        XmlSerializer serializer = new(typeof(List<PictureData>));

        // Open the file for writing or create a new one if it doesn't exist
        using (StreamWriter streamWriter = new("mockFP.xml"))
        {
            // Serialize the entire list at once
            serializer.Serialize(streamWriter, settings.PictureDatas);
        }
        settings.LogService.LogInformation("Settings.PictureData Serialized to mockFP.xml");

        var mockDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "mockFP.xml");
        File.Copy("mockFP.xml", mockDir, true);
    }

    internal static void CopyOutPutDir(Settings settings)
    {
        // first we will clear the directory of any files
        var mockOutPutDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "output");

        var mockDirectories = Directory.GetDirectories(mockOutPutDir);

        foreach (var dir in mockDirectories)
        {
            Directory.Delete(dir, true);
        }

        // Mock copying of outputDir to mockOutPutDir
        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);
        var dirName = Path.GetFileName(settings.OutputDir);
        var makeDir = Path.Combine(mockOutPutDir, dirName);
        Directory.CreateDirectory(makeDir);
        if (Directory.Exists(makeDir))
        {
            foreach (var file in settings.Files)
            {
                var filename = Path.GetFileName(file);
                var writePath = Path.Combine(makeDir, filename);
                File.Copy(file, writePath);
            }
        }
    }

    internal static void CopyTextAddedDir(Settings settings)
    {
        var lastFolderName = Path.GetFileName(settings.TextAddedDir.TrimEnd(Path.DirectorySeparatorChar));

        var mockdir3 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "TextAdded", lastFolderName);

        // if the directory is missing we make it
        if (!Directory.Exists(mockdir3))
        {
            Directory.CreateDirectory(mockdir3);
            settings.LogService.LogInformation("MockDirectory TextAdded was Missing, so it has been created.");
        }

        // we will then clean up the directory if we have residual files there
        DirectoryInfo di3 = new(mockdir3);
        foreach (var file in di3.GetFiles())
        {
            file.Delete();
        }

        // Copy the Text Added Directory to Mock Folder
        var mockdir2 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", settings.TextAddedDir);
        foreach (var file in Directory.GetFiles(settings.TextAddedDir))
        {
            File.Copy(file, $"{mockdir2}/{Path.GetFileName(file)}", true);
        }
    }

    internal static async Task CopyVarietyDir(Settings settings)
    {
        DirectoryInfo di = new(settings.TextAddedDir);

        var di2 = di.GetDirectories();

        foreach (var dir in di2)
        {
            var files = Directory.GetFiles(dir.FullName);
            var textAddedSubDirVideoName = Path.GetFileName(settings.TextAddedDir.TrimEnd(Path.DirectorySeparatorChar));
            var targetDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded", textAddedSubDirVideoName);

            // add the variety directory to the target directory
            Directory.CreateDirectory($"{targetDirectory}/{dir.Name}");

            targetDirectory = Path.Combine(targetDirectory, dir.Name);

            foreach (var file in files)
            {
                var filename = Path.GetFileName(file);

                if (!File.Exists($"{targetDirectory}/{filename}"))
                {
                    await Task.Run(() => File.Copy(file, $"{targetDirectory}/{filename}"));
                }
            }
        }
    }

    internal static async Task SetupFrontPagePictureLineUp(Settings settings)
    {
        // pretend to make line up
        var srcMockFolder = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "output");
        var dirToCopy = Directory.GetDirectories(srcMockFolder);

        foreach (var dir in dirToCopy)
        {
            var targetDir = Path.Combine($"output/{Path.GetFileName(dir)}");
            Directory.CreateDirectory(targetDir);

            var filesToCopy = Directory.GetFiles(dir);

            // copy MockOutputDirs folders to outputdir
            foreach (var outputFile in filesToCopy)
            {
                var completePath = Path.Combine(targetDir, Path.GetFileName(outputFile));
                await Task.Run(() => File.Copy(outputFile, completePath));
            }
        }

        // copy pictures to text added dir
        var sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded");

        var directoryInfos = Directory.GetDirectories(sourceDirectory);

        var lastFolderName = "";
        foreach (var directoryInfo in directoryInfos)
        {
            lastFolderName = Path.GetFileName(directoryInfo.TrimEnd(Path.DirectorySeparatorChar));
        }

        var files = Directory.GetFiles(Path.Combine(sourceDirectory, lastFolderName));

        settings.TextAddedDir = Path.Combine(settings.TextAddedDir, lastFolderName);

        if (!Path.Exists(settings.TextAddedDir))
        {
            Directory.CreateDirectory(settings.TextAddedDir);
        }

        foreach (var file in files)
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
            await settings.LogService.LogError("Could not find mockFP.xml");
            throw;
        }

        List<PictureData> deserializedList;
        // Create a XmlSerializer for the list of PictureData
        XmlSerializer xmlSerializer = new(typeof(List<PictureData>));
        using (StreamReader streamReader = new(srcMockFolder))
        {
            deserializedList = (List<PictureData>)xmlSerializer.Deserialize(streamReader);
        }
        // add the deserialized list to the PictureData list
        // this completes the mocking of the process
        settings.PictureDatas = deserializedList;
    }

    internal static async Task SetupVarietyDisplay(Settings settings)
    {
        // copy pictures to text added dir / var dir
        var sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded");
        var directories = Directory.GetDirectories(sourceDirectory);
        var subDirs = Directory.GetDirectories(directories[0]);

        foreach (var dir in subDirs)
        {
            // this should only have one directory to copy
            // since we are mocking the variety list
            var files = Directory.GetFiles(dir);
            var dirname = Path.GetFileName(dir);
            Directory.CreateDirectory(Path.Combine(settings.TextAddedDir, dirname));
            foreach (var file in files)
            {
                var outPath = Path.Combine(settings.TextAddedDir, dirname);
                outPath = Path.Combine(outPath, Path.GetFileName(file));

                await Task.Run(() => File.Copy(file, outPath));
                await settings.LogService.LogInformation($"Copied {file} to {outPath}");
            }
        }
    }
}
