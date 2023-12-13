using System.IO;
using System.IO.Compression;

namespace Bulk_Thumbnail_Creator.Services
{
    public class ZipService
    {

        static string wwwrootloc = "wwwroot";

        public static string ZipOutputDir()
        {
            string zippedFile = $"{wwwrootloc}/output.zip";

            if (File.Exists(zippedFile))
            {
                File.Delete(zippedFile);
            }

            ZipFile.CreateFromDirectory(Settings.OutputDir, zippedFile);
            return Path.GetFileName(zippedFile);
        }

        public static string ZipVideo()
        {
            string videoname = Path.GetFileName(Settings.PathToVideo);
            string zippedvideo = $"{wwwrootloc}/{videoname}.zip";

            if (File.Exists(zippedvideo))
            {
                File.Delete(zippedvideo);
            }

            using var zip = ZipFile.Open(zippedvideo, ZipArchiveMode.Create);
            if (Settings.Mocking != true)
            {
                zip.CreateEntryFromFile($"{Settings.PathToVideo}", $"{Path.GetFileNameWithoutExtension(Settings.PathToVideo)}");
            }
            return Path.GetFileName(zippedvideo);
        }

        public static string ZipTextAddedDir()
        {
            string zippedtextAddedDir = $"{wwwrootloc}/{Settings.TextAddedDir}.zip";

            if (File.Exists(zippedtextAddedDir))
            {
                File.Delete(zippedtextAddedDir);
            }
            ZipFile.CreateFromDirectory(Settings.TextAddedDir, zippedtextAddedDir);
            return Path.GetFileName(zippedtextAddedDir);
        }

    }
}