using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		public static void GrabSceneScreenshots(string parameters)
		{	// string OGParameters = $" -i " + $@"""{ExtractedFileName}"" " + "-vf " + $@"""select=gt(scene\,0.3)\ """ + " -vsync vfr " + $@"""{fulloutpath}/%03d.png""";

			Process processFFMpeg = new Process();

			processFFMpeg.StartInfo.FileName = Path.GetFullPath("ffmpeg.exe");
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.Arguments = parameters;
			processFFMpeg.StartInfo.UseShellExecute = false;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.StartInfo.RedirectStandardOutput = true;
			
			processFFMpeg.Start();
			processFFMpeg.WaitForExit();
			System.Console.WriteLine("Ffmpeg finished producing pictures");
		}

		public static void RunFFMPG(Dictionary<string,string> parameters, string outPath) 
		{
			var exePars = "";
			foreach (var parameter in parameters) 
			{
				exePars += "-" + parameter.Key;
				exePars +=  " " + parameter.Value + " ";
			}
		

			string path = $@"""{outPath}/%03d.png""";
			exePars += path;
			exePars.TrimEnd(' ');
			// string dontcloseconsole = "/k ";

			GrabSceneScreenshots(exePars);
		}

	}

}
