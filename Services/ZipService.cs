using System.IO;
using System.IO.Compression;

namespace Bulk_Thumbnail_Creator.Services
{
    public class ZipService
    {
        public static void ZipOutputDir()
        {
            ZipFile.CreateFromDirectory(Settings.OutputDir, "output.zip");
        }

        public static void ZipVideo()
        {
            // ZipFile.CreateFromDirectory(Settings.PathToVideo, $"{Path.GetFileName(Settings.PathToVideo)}.zip");
            using var zip = ZipFile.Open($"{Settings.PathToVideo}.zip", ZipArchiveMode.Create);
            zip.CreateEntryFromFile($"{Settings.PathToVideo}", $"{Path.GetFileNameWithoutExtension(Settings.PathToVideo)}");
        }

        public static void ZipTextAddedDir()
        {
            ZipFile.CreateFromDirectory(Settings.TextAddedDir, $"{Settings.TextAddedDir}.zip");
        }

    }
}