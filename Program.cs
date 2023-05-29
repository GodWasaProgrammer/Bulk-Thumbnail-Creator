﻿using System.IO;
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
			string URL = "https://www.youtube.com/watch?v=yppOGpXT998";
			BTCSettings.PathToVideo = await Logic.YouTubeDL(URL);

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

				var bitmap = new Bitmap(file);

				var detectedFacesRect = faceDetector.Forward(bitmap);

				ParamForTextCreation currentParameters = new ParamForTextCreation();

				Point PosOfText = new Point(0, 0);

				Rectangle faceRect;

				if (detectedFacesRect.Length > 0)
				{
					faceRect = detectedFacesRect.First();
					PosOfText = Logic.GettextPosition(bitmap, faceRect);
				}

				currentParameters.PositionOfText = PosOfText;

				currentParameters.WidthOfBox = bitmap.Width;

				currentParameters = Logic.DecideColorGeneration(currentParameters, i);

				currentParameters.FontPointSize = Logic.CalculateFontSize(bitmap.Height);

				Logic.ListOfSettingsForText.Add(Logic.Linear(currentParameters));

				PictureData SavePictureData = new PictureData
				{
					FileName = file,
					ParamForTextCreation = currentParameters,
					IndexOfFile = i
				};

				BTCSettings.PictureDatas.Add(SavePictureData);

				string imageName = Path.GetFileName(file);

				string outputFullPath = Path.GetFullPath(BTCSettings.TextAddedDir) + $"/{imageName}";

				string screenCaptureFile = $"{file}";
				Logic.ProduceTextPictures(i, PosOfText, outputFullPath, screenCaptureFile);
			}

			for (int i = 0; i < BTCSettings.Files.Length; i++)
			{
				var input = BTCSettings.PictureDatas[i];
				Logic.CreateVariety(input);
			}

		}

	}

}
