using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using FaceONNX;
using System.Drawing;
using System;
using UMapx.Visualization;
using System.Diagnostics;
using FFMpegCore;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static async Task Main()
		{
			// YT-DL
			// declare instance of youtubeDL
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "..\\..\\ffmpeg.exe",
				OutputFolder = "YTDL"
			};
			//---------------------------------------------

			BTCSettings.YoutubeLink = "https://www.youtube.com/watch?v=o02f4Ck8F7M";
			// downloads specified video from youtube if it does not already exist.

			if (!File.Exists(BTCSettings.YoutubeLink))
			{
				var res = await ytdl.RunVideoDownload(url: BTCSettings.YoutubeLink);
				
				// sets BTC to run on the recently downloaded file res.data is the returned path.
				BTCSettings.PathToVideo = res.Data;
			};



			Process processFFMpeg = new Process();

			processFFMpeg.StartInfo.FileName = "cmd.exe";
			// processFFMpeg.StartInfo.Arguments = $"-i {BTCSettings.PathToVideo}" + @"-vf select=gt(scene\,0.5)," + "-vsync vfr " + $"YTDL/%03d.png";
			string ffmpeg = ("ffmpeg.exe");
			string v = $" -i ukraine.webm " + "-vf " + @"""select=gt(scene\,0.1)\ """ + " -vsync vfr " + " %03d.png";
			processFFMpeg.StartInfo.Arguments = "/k " + ffmpeg + v;
			processFFMpeg.StartInfo.WorkingDirectory = BTCSettings.YoutubeDLDir;
			processFFMpeg.StartInfo.CreateNoWindow = false;
			processFFMpeg.Start();
			processFFMpeg.WaitForExit();


			// creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
			Directory.CreateDirectory(BTCSettings.OutputDir);
			Directory.CreateDirectory(BTCSettings.TextAddedDir);
			Directory.CreateDirectory(BTCSettings.YoutubeDLDir);

		//	BTCSettings.IntervalBetweenThumbnails = Logic.SplitMetaDataIntoInterValsForThumbNailCreation();


			BTCSettings.IntervalBetweenThumbnails = Logic.ReturnTotalDurationOfClip();

		//	// creates x images from a mediafile
		//	for (int i = 0; i < BTCSettings.NumberOfThumbnails; i++)
		//	{
		//		Logic.ExtractThumbnails(i);
		//		Logic.IncreaseInterval();
		//	}

		//	// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
		//	for (int i = 0; i < BTCSettings.FileNames.Count; i++)
		//	{
		//		// if integer under the number of thumbnails split by 2
		//		if (i < BTCSettings.NumberOfThumbnails / 2 )
		//		{
		//			Logic.ListOfSettingsForText.Add(Logic.GenerateLinearProgressionColorSettings());
		//		}
		//		else
		//		{
		//			Logic.ListOfSettingsForText.Add(Logic.GenerateRandomColorSettings());
		//		}

		//	}

		//	Logic.TextAdder(BTCSettings.TextToAdd);
		//	// Logic.MemeStashDirectories(); not used at the moment but fully functional
		//	Logic.AddTextComposite(BTCSettings.PositionOfText);


		//	var files = Directory.GetFiles(BTCSettings.TextAddedDir, "*.*", SearchOption.AllDirectories);

		//	Directory.CreateDirectory(BTCSettings.FaceDetectionDir);

		//	var faceDetector = new FaceDetector(0.95f, 0.5f);
		//	var painter = new Painter()
		//	{
		//		BoxPen = new Pen(Color.Yellow, 4),
		//		Transparency = 0,
		//	};

		//	Console.WriteLine($"Processing {files.Length} images");

		//	foreach (string file in files)
		//	{
		//		var bitmap = new Bitmap(file);
		//		var output = faceDetector.Forward(bitmap);

		//		foreach (var rectangle in output)
		//		{
		//			var paintData = new PaintData()
		//			{
		//				Rectangle = rectangle,
		//				Title = string.Empty
		//			};
		//			var graphics = Graphics.FromImage(bitmap);
		//			painter.Draw(graphics, paintData);
		//		}

		//		var filename = Path.GetFileName(file);
		//		bitmap.Save(Path.Combine(BTCSettings.FaceDetectionDir, filename));
		//		Console.WriteLine($"Image: [{filename}] --> detected [{output.Length}] faces");
		//	}

		}

	}

}