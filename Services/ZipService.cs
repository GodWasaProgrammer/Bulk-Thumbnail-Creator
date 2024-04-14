using System.IO.Compression;

namespace BulkThumbnailCreator.Services
{
    public static class ZipService
    {
        const string Wwwrootloc = "wwwroot";

        public static string ZipOutputDir(Settings settings)
        {
            if (File.Exists($"{Wwwrootloc}/output.zip"))
            {
                File.Delete($"{Wwwrootloc}/output.zip");
            }

            ZipFile.CreateFromDirectory(settings.OutputDir, $"{Wwwrootloc}/output.zip");
            return Path.GetFileName($"{Wwwrootloc}/output.zip");
        }

        public static string ServeVideo(Settings settings)
        {
            string videoname = Path.GetFileName(settings.PathToVideo);
            return Path.GetFileName(videoname);
        }

        public static string ZipTextAddedDir(Settings settings)
        {
            string zippedtextAddedDir = $"{Wwwrootloc}/textadded.zip";

            if (File.Exists(zippedtextAddedDir))
            {
                File.Delete(zippedtextAddedDir);
            }
            ZipFile.CreateFromDirectory(settings.TextAddedDir, zippedtextAddedDir);
            return Path.GetFileName(zippedtextAddedDir);
        }

        public static string ZipVarietyDir(Settings settings)
        {
            var fetchVarietyDirs = Directory.GetDirectories(settings.TextAddedDir);

            

            // there should only be one varietydirectory
            foreach (var dir in fetchVarietyDirs)
            {
                if (File.Exists($"{Wwwrootloc}/variety.zip"))
                {
                    File.Delete($"{Wwwrootloc}/variety.zip");
                }
                ZipFile.CreateFromDirectory(dir, $"{Wwwrootloc}/variety.zip");
            }
            return Path.GetFileName($"{Wwwrootloc}/variety.zip");
        }
    }
}
