namespace BulkThumbnailCreator.Services
{
    public static class ZipService
    {
        const string Wwwrootloc = "wwwroot";

        public static string ZipOutputDir()
        {
            if (File.Exists($"{Wwwrootloc}/output.zip"))
            {
                File.Delete($"{Wwwrootloc}/output.zip");
            }

            // ZipFile.CreateFromDirectory(Settings.OutputDir, zippedFile);
            return Path.GetFileName($"{Wwwrootloc}/output.zip");
        }

        public static string ZipVideo()
        {
            // string videoname = Path.GetFileName(Settings.PathToVideo);
            // string zippedvideo = $"{wwwrootloc}/{videoname}.zip";

            //if (File.Exists(zippedvideo))
            //{
            //    File.Delete(zippedvideo);
            //}

            //using var zip = ZipFile.Open(zippedvideo, ZipArchiveMode.Create);
            //if (Settings.Mocking != true)
            //{
            //    zip.CreateEntryFromFile($"{Settings.PathToVideo}", $"{Path.GetFileNameWithoutExtension(Settings.PathToVideo)}");
            //}
            //return Path.GetFileName(zippedvideo);
            return null;
        }

        public static string ZipTextAddedDir()
        {
            //string zippedtextAddedDir = $"{wwwrootloc}/{Settings.TextAddedDir}.zip";

            //if (File.Exists(zippedtextAddedDir))
            //{
            //    File.Delete(zippedtextAddedDir);
            //}
            //ZipFile.CreateFromDirectory(Settings.TextAddedDir, zippedtextAddedDir);
            //return Path.GetFileName(zippedtextAddedDir);
            return null;
        }
    }
}
