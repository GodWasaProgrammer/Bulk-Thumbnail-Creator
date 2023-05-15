using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		public static void GrabSceneScreenshots(string parameters)
		{
			//string ExtractedFileName = Path.GetFileName(filename);
			//outputfolder = Path.GetFullPath(outputfolder);
			
			// string OGParameters = $" -i " + $@"""{ExtractedFileName}"" " + "-vf " + $@"""select=gt(scene\,0.{scenedetection})\ """ + " -vsync vfr " + $@"""{outputfolder}/%03d.png""";


			Process processFFMpeg = new Process();
			processFFMpeg.StartInfo.FileName = "ffmpeg.exe";
			processFFMpeg.StartInfo.Arguments = parameters;
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.Start();
			processFFMpeg.WaitForExit();

		}

		public static void RunFFMPG(Dictionary<string,string> parameters) 
		{
			var exePars = "";
			foreach (var parameter in parameters) 
			{
				exePars += "-" + parameter.Key;
				exePars +=  " " + parameter.Value;
			}
			var outpath = Path.GetFullPath(BTCSettings.OutputDir);

			exePars += $" {outpath}/%03d.png";

			GrabSceneScreenshots(exePars);
		}

	}

}
