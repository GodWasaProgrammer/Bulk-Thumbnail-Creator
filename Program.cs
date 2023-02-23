using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Bulk_Thumbnail_Creator;

namespace BTC_Prototype
{
	public class Program
	{
		public static void Main()
		{
			Directory.CreateDirectory(BTCSettings.outputDir);
			Directory.CreateDirectory(BTCSettings.textAddedDir);

			var inputFile = new MediaFile { Filename = "..\\..\\testvideo.mp4" };
			int interval = 15;
			List<string> FilePaths = new List<string>();

			// creates 10 images from a mediafile
			for (int i = 0; i < 10; i++)
			{
				var outputFileName = new MediaFile { Filename = $"output/{i + 1}.jpeg" };

				GrabThumbNail(inputFile, outputFileName, interval);
				interval += 15;

				FilePaths.Add(outputFileName.Filename);
			}

			List<MagickReadSettings> listOfSettingsForText = new List<MagickReadSettings>();


			var settingsTextOne = new MagickReadSettings
			{
				Font = "italic",
				FillColor = MagickColors.Tan,
				StrokeColor = MagickColors.Tan,
				FontStyle = FontStyleType.Bold,
				FontPointsize = 200,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				Height = 1850, // height of text box
				Width = 1500, // width of text box

			};

			listOfSettingsForText.Add(settingsTextOne);

			Random RandomSettings = new Random();
			foreach (string filepath in FilePaths)
			{
				string filepathCorrected = filepath.TrimStart('o', 'u', 't', 'p', 'u', 't', '/');
				string textAddedPath = $"text added/{filepathCorrected}";
				var pathToBackgroundImage = filepath;
				var textToWrite = "WHAT?!";

				// These settings will create a new caption
				// which automatically resizes the text to best
				// fit within the box.

				var settings = listOfSettingsForText[RandomSettings.Next(listOfSettingsForText.Count)];

				using (var image = new MagickImage(pathToBackgroundImage))
				{
					using (var caption = new MagickImage($"label:{textToWrite}", settings))
					{
						// Add the caption layer on top of the background image

						var size = new MagickGeometry(1280, 720);
						image.Composite(caption, 50, 100, CompositeOperator.Over);
						image.Resize(size);
						image.Write(textAddedPath);

					}

				}

			}

		}

		// MediaToolKit
		public static void GrabThumbNail(MediaFile inputFile, MediaFile outputFile, int intervalBetweenPictures)
		{
			using (var engine = new Engine())
			{
				engine.GetMetadata(inputFile);

				var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(intervalBetweenPictures) };

				// fetches image, creates it.
				engine.GetThumbnail(inputFile, outputFile, options);

			}

		}

	}

}