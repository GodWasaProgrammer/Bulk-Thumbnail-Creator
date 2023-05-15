using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;
using FaceONNX;
using System;
using System.Diagnostics;
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

			//FFmpegHandler.RunFFMPG(parameters);

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
			BTCSettings.YoutubeLink = "https://www.youtube.com/watch?v=B66lqt0K2I0";
			var res = await ytdl.RunVideoDownload(url: BTCSettings.YoutubeLink);
			await Console.Out.WriteLineAsync("Download Success:" + res.Success.ToString());

			// sets BTC to run on the recently downloaded file res.data is the returned path.
			BTCSettings.PathToVideo = res.Data;

			if (!BTCSettings.DownloadedVideosList.Contains(BTCSettings.PathToVideo))
			{
				BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);
			}

			//FFmpegHandler.GrabSceneScreenshots(BTCSettings.PathToVideo, 4, BTCSettings.OutputDir);

			var parameters = new Dictionary<string, string>();

			string extractedfilename = Path.GetFileName(BTCSettings.PathToVideo);

			parameters["i"] = $@"""{extractedfilename}""";
			parameters["vf"] = @"select=gt(scene\,0.3)";
			parameters["vsync"] = "vfr";

			FFmpegHandler.RunFFMPG(parameters);

			using (FileStream file = File.Create(BTCSettings.PathToXMLListOfDownloadedVideos))
			{
				serializer.Serialize(file, BTCSettings.DownloadedVideosList);
			}

			string[] SceneFrames = Directory.GetFiles(BTCSettings.OutputDir);

			BTCSettings.FileNames = SceneFrames;

			// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
			for (int i = 0; i < BTCSettings.FileNames.Count(); i++)
			{
				Logic.ListOfSettingsForText.Add(Logic.AdaptableSizeText());
			}

			BTCSettings.ListOfText.Add(BTCSettings.TextToAdd);

			for (int i = 0; i < BTCSettings.FileNames.Count(); i++)
			{
				string fileName = Path.GetFileName(BTCSettings.FileNames[i]);

				string textAddedOutPutFile = Path.GetFullPath(BTCSettings.TextAddedDir) + $"/{fileName}";
				string screenCaptureFile = $"{BTCSettings.FileNames[i]}";

				using (MagickImage outputImage = new MagickImage(screenCaptureFile))
				{
					MagickReadSettings settings = Logic.ListOfSettingsForText[i];
					using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))

					{
						// Add the caption layer on top of the background image
						// gravity will dictate position of your text instead of x/y by a settings var
						outputImage.Composite(caption, BTCSettings.PositionOfText, CompositeOperator.Over);
					
						outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
						outputImage.Quality = 100;
						// outputs the file to the provided path and name
						outputImage.Write(textAddedOutPutFile);

					}

					//await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

					var files = Directory.GetFiles(BTCSettings.TextAddedDir, "*.*", SearchOption.AllDirectories);

					Directory.CreateDirectory(BTCSettings.FaceDetectionDir);

					var faceDetector = new FaceDetector(0.95f, 0.5f);
					var painter = new Painter()
					{
						BoxPen = new Pen(Color.Yellow, 4),
						Transparency = 0,
					};

					Console.WriteLine($"Processing {files.Length} images");

					foreach (string file in files)
					{
						var bitmap = new Bitmap(file);
						var output = faceDetector.Forward(bitmap);

						foreach (var rectangle in output)
						{
							//var imgWidth = 640;
							//var imgHeight = 480;

							//var Width = rectangle.Location;

							//var originX = rectangle.X;
							//var originY = rectangle.Y; //20

							//var height = rectangle.Height; //100

							var paintData = new PaintData()
							{
								Rectangle = rectangle,
								Title = string.Empty
							};
							var graphics = Graphics.FromImage(bitmap);
							painter.Draw(graphics, paintData);
						}

						var filename = Path.GetFileName(file);
						bitmap.Save(Path.Combine(BTCSettings.FaceDetectionDir, filename));
						Console.WriteLine($"Image: [{filename}] --> detected [{output.Length}] faces");
					}

				}

			}

		}
	}
}
