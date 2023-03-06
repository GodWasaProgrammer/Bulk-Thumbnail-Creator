using ImageMagick;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{

	internal class Logic
	{
		public static void AddTextComposite()
		{
			

				for (int i = 0; i < BTCSettings.FilePaths.Count; i++)
				{
					string filepath = BTCSettings.FilePaths[i];
					string filepathCorrected = filepath.TrimStart('o', 'u', 't', 'p', 'u', 't', '/');
					string textAddedPath = $"text added/{filepathCorrected}";
					var pathToBackgroundImage = filepath;

					using (MagickImage image = new MagickImage(pathToBackgroundImage))
					{
						MagickReadSettings settings = Logic.listOfSettingsForText[i];
						using (var caption = new MagickImage($"caption:{BTCSettings.listOfText[0]}", settings))
						using (var caption2 = new MagickImage($"caption:{BTCSettings.listOfText[1]}", settings))
						{
							// Add the caption layer on top of the background image

							var size = new MagickGeometry(1280, 720);

							image.Composite(caption, BTCSettings.positionoftextonHorizontalAxis, BTCSettings.positionoftextonVerticalAxis, CompositeOperator.Over);
							image.Composite(caption2, BTCSettings.positionoftextonHorizontalAxis, BTCSettings.LowerPositionHorizontalAxis, CompositeOperator.Over);
							image.Resize(size);
							image.Write(textAddedPath);
						}

					}

				}

		}

		public static void ExtractThumbnails(int i)
		{
			var inputFile = new MediaFile { Filename = BTCSettings.pathToVideo };

			var outputFileName = new MediaFile { Filename = $"output/{i + 1}.jpeg" };

			GrabThumbNail(inputFile, outputFileName, BTCSettings.intervalBetweenThumbnails);

			// increases the interval between pictures by 15 seconds
			BTCSettings.IncreaseInterval();

			BTCSettings.FilePaths.Add(outputFileName.Filename);
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

		// list of created Presets for printing text to image
		public static List<MagickReadSettings> listOfSettingsForText = new List<MagickReadSettings>();

		static readonly Random colorRandom = new Random();

		internal static MagickColor RandomizeColor()
		{
			byte pickedColorRedRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorGreenRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorBlueRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			// byte pickedColorAlphaRGB = (byte)colorRandom.Next(MaxRGB);

			MagickColor colorRNGPicked;

			colorRNGPicked = MagickColor.FromRgba(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB, 255);

			return colorRNGPicked;
		}

		public static MagickReadSettings GenerateColorSettings()
		{

			MagickReadSettings settingsTextRandom = new MagickReadSettings
			{
				Font = "italic",
				FillColor = RandomizeColor(),
				StrokeColor = RandomizeColor(),
				BorderColor = RandomizeColor(),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 10,
				FontPointsize = 200,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				Height = 1850, // height of text box
				Width = 1900, // width of text box
			};

			return settingsTextRandom;
		}

	}
}
