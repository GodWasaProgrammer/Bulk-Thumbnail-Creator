using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator
{
	internal class FFmpegHandler
	{
		/// <summary>
		/// This runs the process of ffmpeg and feeds it with parameters for file extraction
		/// </summary>
		/// <param name="parameters"></param>
		private static async Task ExecuteFFMPG(string parameters)
		{
            Process processFFMpeg = new();

			processFFMpeg.StartInfo.FileName = Settings.FfmpegDir;
			processFFMpeg.StartInfo.WorkingDirectory = Settings.YTDLOutPutDir;
			processFFMpeg.StartInfo.Arguments = parameters;
			processFFMpeg.StartInfo.UseShellExecute = false;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.StartInfo.RedirectStandardOutput = true;

			bool ProcessStarted = processFFMpeg.Start();
			
			if (!ProcessStarted)
			{
                Settings.LogService.LogError("FFMpeg failed to start");
                return;
            }
			else
			{
                Settings.LogService.LogInformation("FFMpeg started");
            }

			await Task.Run(() => processFFMpeg.WaitForExit());

			Settings.LogService.LogInformation("Ffmpeg finished producing pictures");
		}

		/// <summary>
		/// This runs FFMpeg.exe and extracts the files requested based on the CLI input
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="outPath"></param>
		public static async Task RunFFMPG(Dictionary<string,string> parameters, string outPath) 
		{
			var exePars = "";
			foreach (var parameter in parameters) 
			{
				exePars += "-" + parameter.Key;
				exePars +=  " " + parameter.Value + " ";
			}

			exePars += outPath;
			await ExecuteFFMPG(exePars);
		}

	}

}