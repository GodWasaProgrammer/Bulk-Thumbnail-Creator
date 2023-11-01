using System.IO;
using System.IO.Compression;

namespace Bulk_Thumbnail_Creator.Services
{
    public class ZipService
    {
        public static string ZipOutputDir()
        {
            string zippedFile = "output.zip";

            ZipFile.CreateFromDirectory(Settings.OutputDir, zippedFile);

            return zippedFile;
        }

        public static string ZipVideo()
        {
            string zippedvideo = $"{Settings.PathToVideo}.zip";
            using var zip = ZipFile.Open(zippedvideo, ZipArchiveMode.Create);
            zip.CreateEntryFromFile($"{Settings.PathToVideo}", $"{Path.GetFileNameWithoutExtension(Settings.PathToVideo)}");

            return zippedvideo;
        }

        public static string ZipTextAddedDir()
        {
            string zippedtextAddedDir = $"{Settings.TextAddedDir}.zip";
            ZipFile.CreateFromDirectory(Settings.TextAddedDir, $"{Settings.TextAddedDir}.zip");

            return zippedtextAddedDir;
        }

    }
}