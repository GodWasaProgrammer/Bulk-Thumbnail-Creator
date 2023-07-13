using System.IO;
using System.Threading.Tasks;
using FaceONNX;
using System;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using System.Drawing;
using System.Xml.Schema;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static async Task Main()
		{
			// creates our 3 dirs to push out unedited thumbnails, and the edited thumbnails and also a path for where the downloaded youtube clips goes.
			Logic.CreateDirectories(BTCSettings.OutputDir, BTCSettings.TextAddedDir, BTCSettings.YoutubeDLDir);

			// adds a few strings to the list of text to be printed
			string textToPrint = "Diablo 4\nButchers Darling\nButcher be like:FRESH MEAT!";
			BTCSettings.ListOfText.Add(textToPrint);
			string textToPrint2 = "Diablo 4\nServer Slam!\nFresh Meat!";
			BTCSettings.ListOfText.Add(textToPrint2);
			string textToPrint3 = "Diablo 4\nThe Latest Hack'N'Slash!\nPlay now!";
			BTCSettings.ListOfText.Add(textToPrint3);
			string textToPrint4 = "Bulk Thumbnail Creator\nYour Personalized Thumbnails!\nTry it, its free forever!";
			BTCSettings.ListOfText.Add(textToPrint4);

			BTCSettings.DownloadedVideosList = Logic.DeSerializeXMLToListOfStrings(BTCSettings.PathToXMLListOfDownloadedVideos);

			//downloads the specified url
			string URL = "https://youtube.com/live/tqsYO976zTU";

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
			string truePath = Path.GetFullPath(BTCSettings.OutputDir);
			string pictureOutput = $@"""{truePath}/%03d.png""";

			FFmpegHandler.RunFFMPG(parameters, pictureOutput);
			#endregion

			Logic.SerializeListOfStringsToXML(BTCSettings.PathToXMLListOfDownloadedVideos, BTCSettings.DownloadedVideosList);

			BTCSettings.Files = Directory.GetFiles(BTCSettings.OutputDir, "*.*", SearchOption.AllDirectories);

			var faceDetector = new FaceDetector(0.95f, 0.5f);

			Console.WriteLine($"Processing {BTCSettings.Files.Length} images");

			// main loop for detecting faces, placing text where face is not
			// outputting file
			for (int fileIndex = 0; fileIndex < BTCSettings.Files.Length; fileIndex++)
			{
				string file = BTCSettings.Files[fileIndex];

				Bitmap bitmap = new Bitmap(file);

				Rectangle[] detectedFacesRect = faceDetector.Forward(bitmap);

				ParamForTextCreation currentParameters = new ParamForTextCreation();

				Rectangle faceRect = detectedFacesRect.FirstOrDefault();
				currentParameters = Logic.GettextPosition(currentParameters, bitmap, faceRect);

				currentParameters = Logic.DecideColorGeneration(currentParameters, fileIndex);

				currentParameters.Font = Logic.PickRandomFont();

				// picks a random string from the list
				Random pickAString = new Random();
				int pickedString = pickAString.Next(BTCSettings.ListOfText.Count);
				currentParameters.Text = BTCSettings.ListOfText[pickedString];

				MagickReadSettings settings = Logic.TextSettingsGeneration(currentParameters);

				PictureData PassPictureData = new PictureData()
				{
					FileName = file,
					ParamForTextCreation = currentParameters,
					ReadSettings = settings,
				};

				BTCSettings.PictureDatas.Add(PassPictureData);
				//Logic.ProduceTextPictures(PassPictureData);
			}

			#region Make Showcase Video
			//Dictionary<string, string> paramToMakeVideoOfResult = new Dictionary<string, string>();
			//paramToMakeVideoOfResult["framerate"] = "2";
			//paramToMakeVideoOfResult["i"] = $@"""{Path.GetFullPath(BTCSettings.TextAddedDir)}/%03d.png""";
			//string getTruePath = Path.GetFullPath(BTCSettings.TextAddedDir);
			//string showCaseVideoOutPut = $@"""{getTruePath}/showcase.mp4""";

			//FFmpegHandler.RunFFMPG(paramToMakeVideoOfResult, showCaseVideoOutPut);
			#endregion

			// just to try out variety will be on interaction/choice of pic
			for (int i = 0; i < BTCSettings.PictureDatas.Count; i++)
			{
				var input = BTCSettings.PictureDatas[i];
				Logic.ProduceSaturationVarietyData(input);

				Logic.ProduceFontVarietyData(input);

				Logic.ProducePlacementOfTextVarietyData(input);
			}

			for (int i = 0; i < BTCSettings.PictureDatas[0].Varieties.Count; i++)
			{
				Logic.ProduceTextPictures(BTCSettings.PictureDatas[0].Varieties[i]);
			}

		}

	}

}
