using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using FaceONNX;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using UMapx.Visualization;
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

			// YT-DL
			// declare instance of youtubeDL
			var ytdl = new YoutubeDL
			{
				// set paths
				YoutubeDLPath = "..\\..\\yt-dlp.exe",
				FFmpegPath = "YTDL/ffmpeg.exe",
				OutputFolder = "YTDL"
			};

			XmlSerializer serializer = new XmlSerializer(typeof(List<string>));

			if (File.Exists(BTCSettings.PathToXMLListOfDownloadedVideos))
			{
				using (FileStream file = File.OpenRead(BTCSettings.PathToXMLListOfDownloadedVideos))
				{
					BTCSettings.DownloadedVideosList = (List<string>)serializer.Deserialize(file);
				}

			}

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

			using (FileStream file = File.Create(BTCSettings.PathToXMLListOfDownloadedVideos))
			{
				serializer.Serialize(file, BTCSettings.DownloadedVideosList);
			}

			var files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

			Directory.CreateDirectory(BTCSettings.FaceDetectionDir);

			var faceDetector = new FaceDetector(0.95f, 0.5f);


			Console.WriteLine($"Processing {files.Length} images");


			foreach (string file in files)
			{
				var bitmap = new Bitmap(file);
				var detectedFacesRect = faceDetector.Forward(bitmap);

				ParamForTextCreation currentParameters = new ParamForTextCreation();

				// if rectangle didnt output anything, will put text default
				// otherwise foreach rectangle determine where to put text

				var faceRect = detectedFacesRect.First();

				var LocationOfRectangleCenterYpos = faceRect.Y + faceRect.Height / 2;

				// should calculate distance between position of the rectangle with a face in it and the 0,0 pos and max pos

				var sourceIMGMiddleY = bitmap.Height / 2;

				// var bmpheightminusrectYpos = bitmap.Height - locationofrectangleY;

				Point PosOfText;

				if (LocationOfRectangleCenterYpos > sourceIMGMiddleY)
				{
					// make text appear on lower half
					// need to be fed to the Textadding function
					// create a list of the detected faces, output pos of text to list, then read from textadding function
					PosOfText = new Point(0, 0);	
				}
				else
				{
					// make text appear on upper half
					// need to be fed to the TextAdding function
					PosOfText = new Point(0, sourceIMGMiddleY);
				}

				currentParameters.PositionOfText = PosOfText;
				BTCSettings.PositionOfText = PosOfText; // feed position

				#region draw face boundary
				var paintData = new PaintData()
				{
					Rectangle = faceRect,
					Title = string.Empty
				};
				var painter = new Painter()
				{
					BoxPen = new Pen(Color.Yellow, 4),
					Transparency = 0,
				};
				var graphics = Graphics.FromImage(bitmap);
				painter.Draw(graphics, paintData);
				#endregion

				var filename = Path.GetFileName(file);
				bitmap.Save(Path.Combine(BTCSettings.FaceDetectionDir, filename));
				Console.WriteLine($"Image: [{filename}] --> detected [{detectedFacesRect.Length}] faces");
			}

			string[] SceneFrames = Directory.GetFiles(BTCSettings.OutputDir);

			BTCSettings.FileNames = SceneFrames;

			float hueFillColor = 0F;
			float saturationFillColor = 1F;
			float lightnessFillColor = 0.50F;

			float hueStrokeColor = 125F;
			float saturationStrokeColor = 1F;
			float lightnessStrokeColor = 0.50F;

			float hueBorderColor = 28F;
			float saturationBorderColor = 1F;
			float lightnessBorderColor = 0.50F;

			// loops for each file in filenames, generate some settings, return the settings, add em to our listofsettingsfortext.
			for (int i = 0; i < BTCSettings.FileNames.Count(); i++)
			{

				ParamForTextCreation imageObject = new ParamForTextCreation
				{
					Filename = BTCSettings.FileNames[i],
					DirectoryOfFile = BTCSettings.FileNames[i]

				};

				if (i > 1)
				{
					hueFillColor += +12.5F;
					hueStrokeColor += +12.5F;

					if (i > 5)
					{
						hueBorderColor += 10F;
					}

					if (hueFillColor > 360)
					{
						hueFillColor = 0F;
					}

					if (hueStrokeColor > 360)
					{
						hueStrokeColor = 0F;
					}

					if (hueBorderColor > 360)
					{
						hueBorderColor = 0F;
					}

					// hueStrokeColor = hueStrokeColor + 25;

					imageObject.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);

					imageObject.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);

					imageObject.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);

				}
				else
				{
					imageObject.FillColor.SetByHSL(hueFillColor, saturationFillColor, lightnessFillColor);
					imageObject.StrokeColor.SetByHSL(hueStrokeColor, saturationStrokeColor, lightnessStrokeColor);
					imageObject.BorderColor.SetByHSL(hueBorderColor, saturationBorderColor, lightnessBorderColor);
				}

				Logic.ListOfSettingsForText.Add(Logic.Linear(imageObject));
			}

			BTCSettings.ListOfText.Add(BTCSettings.TextToAdd);

			for (int i = 0; i < BTCSettings.FileNames.Count(); i++)
			{
				string imageName = Path.GetFileName(BTCSettings.FileNames[i]);

				string outputFullPath = Path.GetFullPath(BTCSettings.TextAddedDir) + $"/{imageName}";
				string screenCaptureFile = $"{BTCSettings.FileNames[i]}";

				using (MagickImage outputImage = new MagickImage(screenCaptureFile))
				{
					MagickReadSettings settings = Logic.ListOfSettingsForText[i];
					using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))
					{
						// Add the caption layer on top of the background image

						outputImage.Composite(caption, BTCSettings.PositionOfText.X, BTCSettings.PositionOfText.Y, CompositeOperator.Over);

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
