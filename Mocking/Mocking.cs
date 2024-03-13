namespace BulkThumbnailCreator;

internal class Mocking
{
    // this class is displayed in the order it is called for easy understanding of its structure
    public static int BTCRunCount = 0;

    internal static void SerializePicData(Settings settings)
    {
        XmlSerializer Serializer = new(typeof(List<PictureData>));

        // Open the file for writing or create a new one if it doesn't exist
        using (StreamWriter streamWriter = new("mockFP.xml"))
        {
            // Serialize the entire list at once
            Serializer.Serialize(streamWriter, settings.PictureDatas);
        }
        settings.LogService.LogInformation("Settings.PictureDatas Serialized to mockFP.xml");

        string mockDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "mockFP.xml");
        File.Copy("mockFP.xml", mockDir, true);
    }

    internal static void CopyOutPutDir(Settings settings)
    {
        // first we will clear the directory of any files
        string mockOutPutDir = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "output");

        string[] mockDirectories = Directory.GetDirectories(mockOutPutDir);

        foreach (string dir in mockDirectories)
        {
            Directory.Delete(dir, true);
        }

        // Mock copying of outputDir to mockOutPutDir
        settings.Files = Directory.GetFiles(settings.OutputDir, "*.*", SearchOption.AllDirectories);
        string dirName = Path.GetFileName(settings.OutputDir);
        string makeDir = Path.Combine(mockOutPutDir, dirName);
        Directory.CreateDirectory(makeDir);
        if (Directory.Exists(makeDir))
        {
            foreach (var file in settings.Files)
            {
                string filename = Path.GetFileName(file);
                string writePath = Path.Combine(makeDir, filename);
                File.Copy(file, writePath);
            }
        }
    }

    internal static void CopyTextAddedDir(Settings settings)
    {
        string lastFolderName = Path.GetFileName(settings.TextAddedDir.TrimEnd(Path.DirectorySeparatorChar));

        string mockdir3 = Path.Combine("..", "Mocking", "FrontpagePictureLineUp", "TextAdded", lastFolderName);

        // if the directory is missing we make it
        if (!Directory.Exists(mockdir3))
        {
            Directory.CreateDirectory(mockdir3);
            settings.LogService.LogInformation("MockDirectory TextAdded was Missing, so it has been created.");
        }

        // we will then clean up the directory if we have residual files there
        DirectoryInfo di3 = new(mockdir3);
        foreach (FileInfo file in di3.GetFiles())
        {
            file.Delete();
        }

        // Copy the Text Added Directory to Mock Folder
        string mockdir2 = Path.Combine($"..", "Mocking", "FrontpagePictureLineUp", settings.TextAddedDir);
        string[] Mockfiles = Directory.GetFiles(settings.TextAddedDir);
        foreach (string file in Mockfiles)
        {
            File.Copy(file, $"{mockdir2}/{Path.GetFileName(file)}", true);
        }
    }

    internal static async Task CopyVarietyDir(Settings settings)
    {
        DirectoryInfo di = new(settings.TextAddedDir);

        DirectoryInfo[] di2 = di.GetDirectories();

        foreach (DirectoryInfo dir in di2)
        {
            string[] files = Directory.GetFiles(dir.FullName);
            
            string varFolderName = Path.GetFileName(dir.FullName.TrimEnd(Path.DirectorySeparatorChar));
            string textAddedSubDirVideoName = Path.GetFileName(settings.TextAddedDir.TrimEnd(Path.DirectorySeparatorChar));
            string TargetDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded", textAddedSubDirVideoName);

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

    internal static async Task SetupFrontPagePictureLineUp(Settings settings)
    {
        // pretend to make line up
        string srcMockFolder = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "output");
        string[] dirToCopy = Directory.GetDirectories(srcMockFolder);

        

        foreach (string dir in dirToCopy)
        {
            string targetDir = Path.Combine($"output/{Path.GetFileName(dir)}");
            Directory.CreateDirectory(targetDir);

            string[] filesToCopy = Directory.GetFiles(dir);

            // copy MockOutputDirs folders to outputdir
            foreach (string outputFile in filesToCopy)
            {
                string completePath = Path.Combine(targetDir, Path.GetFileName(outputFile));
                await Task.Run(() => File.Copy(outputFile, completePath));
            }
        }

        // copy pictures to text added dir
        string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded");

        string[] directoryInfos = Directory.GetDirectories(sourceDirectory);

        string lastFolderName = "";
        foreach (string directoryInfo in directoryInfos)
        {
            lastFolderName = Path.GetFileName(directoryInfo.TrimEnd(Path.DirectorySeparatorChar));
        }

        string[] files = Directory.GetFiles(Path.Combine(sourceDirectory, lastFolderName));

        settings.TextAddedDir = Path.Combine(settings.TextAddedDir, lastFolderName);

        if(!Path.Exists(settings.TextAddedDir))
        {
            Directory.CreateDirectory(settings.TextAddedDir);
        }

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
        // add the deserialized list to the PictureDatas list
        // this completes the mocking of the process
        settings.PictureDatas = deserializedList;
    }

    internal static async Task SetupVarietyDisplay(Settings settings)
    {
        // copy pictures to text added dir / var dir
        string sourceDirectory = Path.Combine(Path.GetFullPath(".."), "Mocking", "FrontpagePictureLineUp", "TextAdded");
        string[] Directories = Directory.GetDirectories(sourceDirectory);
        string[] subDirs = Directory.GetDirectories(Directories[0]);

        foreach (string dir in subDirs)
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
                await settings.LogService.LogInformation($"Copied {file} to {outPath}");
            }
        }
    }

}