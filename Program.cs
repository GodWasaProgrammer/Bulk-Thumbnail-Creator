using System.IO;
using System.Threading.Tasks;
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
			Logic.CreateDirectories(BTCSettings.OutputDir, BTCSettings.TextAddedDir, BTCSettings.YoutubeDLDir);

			// adds a single string to the list of text to be printed
			BTCSettings.ListOfText.Add(BTCSettings.TextToAdd);

			BTCSettings.DownloadedVideosList = Logic.DeSerializeXMLToListOfStrings(BTCSettings.PathToXMLListOfDownloadedVideos);

			//downloads the specified url
			string URL = "https://www.youtube.com/watch?v=ZnQdseqFjj0";

			BTCSettings.PathToVideo = await Logic.YouTubeDL(URL);
			BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);

			// Adds To DownloadedVideosList if it is not already containing it
			if (!BTCSettings.DownloadedVideosList.Contains(BTCSettings.PathToVideo))
			{
				BTCSettings.DownloadedVideosList.Add(BTCSettings.PathToVideo);
			}

			#region Run FfMpeg
			var parameters = new Dictionary<string, string>();

			string extractedfilename = Path.GetFileName(BTCSettings.PathToVideo);

			parameters["i"] = $@"""{extractedfilename}""";
			parameters["vf"] = @"select=gt(scene\,0.3)";
			parameters["vsync"] = "vfr";

			FFmpegHandler.RunFFMPG(parameters, Path.GetFullPath(BTCSettings.OutputDir));
			#endregion

			Logic.SerializeListOfStringsToXML(BTCSettings.PathToXMLListOfDownloadedVideos, BTCSettings.DownloadedVideosList);

			BTCSettings.Files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

			var faceDetector = new FaceDetector(0.95f, 0.5f);

			Console.WriteLine($"Processing {BTCSettings.Files.Length} images");

			// main loop for detecting faces, placing text where face is not
			// outputting file
			for (int i = 0; i < BTCSettings.Files.Length; i++)
			{
				string file = BTCSettings.Files[i];

				Bitmap bitmap = new Bitmap(file);

				Rectangle[] detectedFacesRect = faceDetector.Forward(bitmap);

				ParamForTextCreation currentParameters = new ParamForTextCreation();

				Point PosOfText;
				if (detectedFacesRect.Length > 0)
				{
					Rectangle faceRect = detectedFacesRect.First();
					PosOfText = Logic.GettextPosition(bitmap, faceRect);
				}
				else
				{
					Rectangle EmptyRectangle = new Rectangle(bitmap.Width, bitmap.Height, 50, 50);
					PosOfText = Logic.GettextPosition(bitmap, EmptyRectangle);
				}

				currentParameters.PositionOfText = PosOfText;

				currentParameters.WidthOfBox = bitmap.Width;

				currentParameters = Logic.DecideColorGeneration(currentParameters, i);

				currentParameters.FontPointSize = Logic.CalculateFontSize(bitmap.Height);

				currentParameters.Font = Logic.PickRandomFont();

				// placeholder.
				currentParameters.Text = BTCSettings.ListOfText[0];

				MagickReadSettings settings = Logic.TextSettingsGeneration(currentParameters);

				PictureData PassPictureData = new PictureData
				{
					FileName = file,
					ParamForTextCreation = currentParameters,
					ReadSettings = settings,
				};

				BTCSettings.PictureDatas.Add(PassPictureData);

				string imageName = Path.GetFileName(file);

				string outputFullPath = Path.GetFullPath(BTCSettings.TextAddedDir) + $"/{imageName}";

				Logic.ProduceTextPictures(PassPictureData, outputFullPath);
			}

			//// just to try out variety will be on interaction/choice of pic
			//for (int i = 0; i < BTCSettings.Files.Length; i++)
			//{
			//	var input = BTCSettings.PictureDatas[i];
			//	Logic.CreateVariety(input, BTCSettings.TextAddedDir);
			//}

		}

	}

}
