using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		/// <summary>
		/// This runs the process of ffmpeg and feeds it with parameters for file extraction
		/// </summary>
		/// <param name="parameters"></param>
		private static void ExecuteFFMPG(string parameters)
		{
            Process processFFMpeg = new();

			processFFMpeg.StartInfo.FileName = BTCSettings.FfmpegDir;
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.Arguments = parameters;
			processFFMpeg.StartInfo.UseShellExecute = false;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.StartInfo.RedirectStandardOutput = true;

			processFFMpeg.Start();
			processFFMpeg.WaitForExit();
			System.Console.WriteLine("Ffmpeg finished producing pictures");
		}

		/// <summary>
		/// This runs FFMpeg.exe and extracts the files requested based on the CLI input
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="outPath"></param>
		public static void RunFFMPG(Dictionary<string,string> parameters, string outPath) 
		{
			var exePars = "";
			foreach (var parameter in parameters) 
			{
				exePars += "-" + parameter.Key;
				exePars +=  " " + parameter.Value + " ";
			}

			exePars += outPath;
			ExecuteFFMPG(exePars);
		}

	}

}
