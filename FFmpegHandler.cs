using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		public static void GrabSceneScreenshots(string parameters)
		{
			string ExtractedFileName = Path.GetFileName(BTCSettings.PathToVideo);
			string fulloutpath = Path.GetFullPath(BTCSettings.OutputDir);
			
			string OGParameters = $" -i " + $@"""{ExtractedFileName}"" " + "-vf " + $@"""select=gt(scene\,0.3)\ """ + " -vsync vfr " + $@"""{fulloutpath}/%03d.png""";

			Process processFFMpeg = new Process();

			processFFMpeg.StartInfo.FileName = "ffmpeg.exe";
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.Arguments = parameters;
			//processFFMpeg.StartInfo.UseShellExecute = false;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			//processFFMpeg.StartInfo.RedirectStandardOutput = true;

			processFFMpeg.Start();
			processFFMpeg.WaitForExit();

			//while (!processFFMpeg.StandardOutput.EndOfStream)
			//{
			//	string line = processFFMpeg.StandardOutput.ReadLine();
			//	Console.WriteLine(line);
			//	// do something with line
			//}

		}

		public static void RunFFMPG(Dictionary<string,string> parameters) 
		{
			var exePars = "";
			foreach (var parameter in parameters) 
			{
				exePars += "-" + parameter.Key;
				exePars +=  " " + parameter.Value + " ";
			}
			var outpath = Path.GetFullPath(BTCSettings.OutputDir);

			string path = $@"""{outpath}/%03d.png""";
			exePars += path;
			exePars.TrimEnd(' ');
			// string dontcloseconsole = "/k ";

			GrabSceneScreenshots(exePars);
		}

	}

}
