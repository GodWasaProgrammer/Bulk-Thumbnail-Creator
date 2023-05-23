using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using FaceONNX;
using System;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using System.Drawing;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static async Task Main()
		{
			// creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
			Directory.CreateDirectory(BTCSettings.OutputDir);
			Directory.CreateDirectory(BTCSettings.TextAddedDir);
			Directory.CreateDirectory(BTCSettings.YoutubeDLDir);

			// adds a single string to the list of text to be printed
			BTCSettings.ListOfText.Add(BTCSettings.TextToAdd);

			// YT-DL
			// declare instance of youtubeDL
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "YTDL/ffmpeg.exe",
				OutputFolder = "YTDL"
			};

			Logic.DeSerializeDownloadedVideosList();

			// downloads specified video from youtube if it does not already exist.
			BTCSettings.YoutubeLink = "https://www.youtube.com/watch?v=yppOGpXT998";
			var res = await ytdl.RunVideoDownload(url: BTCSettings.YoutubeLink);
			await Console.Out.WriteLineAsync("Download Success:" + res.Success.ToString());

			// sets BTC to run on the recently downloaded file res.data is the returned path.
			BTCSettings.PathToVideo = res.Data;

			if (!BTCSettings.DownloadedVideosList.Contains(BTCSettings.PathToVideo))
			{
				BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);
			}

			var parameters = new Dictionary<string, string>();

			string extractedfilename = Path.GetFileName(BTCSettings.PathToVideo);

			parameters["i"] = $@"""{extractedfilename}""";
			parameters["vf"] = @"select=gt(scene\,0.3)";
			parameters["vsync"] = "vfr";

			FFmpegHandler.RunFFMPG(parameters, Path.GetFullPath(BTCSettings.OutputDir));

			Logic.SerializeDownloadedVideosList();

			var files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

			var faceDetector = new FaceDetector(0.95f, 0.5f);

			Console.WriteLine($"Processing {files.Length} images");

			// main loop for detecting faces, placing text where face is not
			// outputting file
			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i];
				var bitmap = new Bitmap(file);
				var detectedFacesRect = faceDetector.Forward(bitmap);

				ParamForTextCreation currentParameters = new ParamForTextCreation();

				Point PosOfText = new Point(0, 0);

				Rectangle faceRect;

				if (detectedFacesRect.Length > 0)
				{
					faceRect = detectedFacesRect.First();

					var LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / 2;

					var sourceIMGMiddleY = bitmap.Height / 2;

					// var bmpheightminusrectYpos = bitmap.Height - locationofrectangleY;

					if (sourceIMGMiddleY > LocationOfRectangleCenterYpos)
					{
						// make text appear on lower half
						PosOfText = new Point(0, bitmap.Height - 300);
					}
					else
					{
						// make text appear on upper half
						// need to be fed to the TextAdding function
						PosOfText = new Point(0, sourceIMGMiddleY);
					}

				}

				currentParameters.PositionOfText = PosOfText;

				currentParameters = Logic.DecideColorGeneration(currentParameters, i);

				Logic.ListOfSettingsForText.Add(Logic.Linear(currentParameters));

				string imageName = Path.GetFileName(file);

				string outputFullPath = Path.GetFullPath(BTCSettings.TextAddedDir) + $"/{imageName}";

				string screenCaptureFile = $"{file}";

				using (MagickImage outputImage = new MagickImage(screenCaptureFile))
				{
					MagickReadSettings settings = Logic.ListOfSettingsForText[i];
					using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))
					{
						// Add the caption layer on top of the background image
						outputImage.Composite(caption, PosOfText.X, PosOfText.Y, CompositeOperator.Over);

						outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
						outputImage.Quality = 100;
						// outputs the file to the provided path and name
						outputImage.Write(outputFullPath);
					}

				}

			}

		}

	}

}
