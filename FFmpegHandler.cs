using System.Diagnostics;
using System.IO;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		public static void RunFFMpeg(string filename, float scenedetection, string outputfolder)
		{
			string ExtractedFileName = Path.GetFileName(filename);
			outputfolder = Path.GetFullPath(outputfolder);
			
			string Parameters = $" -i " + $@"""{ExtractedFileName}"" " + "-vf " + @"""select=gt(scene\,0.4)\ """ + " -vsync vfr " + $@"""{outputfolder}/%03d.png""";

			Process processFFMpeg = new Process();
			processFFMpeg.StartInfo.FileName = "ffmpeg.exe";
			processFFMpeg.StartInfo.Arguments = Parameters;
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.Start();
			processFFMpeg.WaitForExit();

		}
	}
}
