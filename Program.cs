using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.IO;
using ImageMagick;
using Bulk_Thumbnail_Creator;

namespace BTC_Prototype
{
	public class Program
	{
		static readonly Random RandomSettings = new Random();
		public static void Main()
		{
			Directory.CreateDirectory(BTCSettings.outputDir);
			Directory.CreateDirectory(BTCSettings.textAddedDir);


			var inputFile = new MediaFile { Filename = BTCSettings.pathToVideo };

			// creates 10 images from a mediafile
			for (int i = 0; i < 25; i++)
			{
				var outputFileName = new MediaFile { Filename = $"output/{i + 1}.jpeg" };

				GrabThumbNail(inputFile, outputFileName, BTCSettings.intervalBetweenThumbnails);

				// increases the interval between pictures by 15 seconds
				BTCSettings.IncreaseInterval();

				BTCSettings.FilePaths.Add(outputFileName.Filename);
			}

			BTCSettings.GenerateColorSettings();

			int x = 0;

			foreach (string filepath in BTCSettings.FilePaths)
			{
				string filepathCorrected = filepath.TrimStart('o', 'u', 't', 'p', 'u', 't', '/');
				string textAddedPath = $"text added/{filepathCorrected}";
				var pathToBackgroundImage = filepath;
				var textToWrite = "I beg Your Pardon?";
				using (var image = new MagickImage(pathToBackgroundImage))
				{
					MagickReadSettings settings = BTCSettings.listOfSettingsForText[x];
					using (var caption = new MagickImage($"label:{textToWrite}", settings))
					{
						// Add the caption layer on top of the background image

						var size = new MagickGeometry(1280, 720);
						image.Composite(caption, BTCSettings.positionoftextonHorizontalAxis, BTCSettings.positionoftextonVerticalAxis, CompositeOperator.Over);
						image.Composite(caption, BTCSettings.positionoftextonHorizontalAxis, BTCSettings.LowerPositionHorizontalAxis, CompositeOperator.Over);
						image.Resize(size);
						image.Write(textAddedPath);
					}
					x++;
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