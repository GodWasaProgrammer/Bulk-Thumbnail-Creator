using ImageMagick;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bulk_Thumbnail_Creator
{

	internal class Logic
	{
		public static void AddTextComposite()
		{
			for (int i = 0; i < BTCSettings.FileNames.Count; i++)
			{
				string fileName = BTCSettings.FileNames[i];

				string textAddedPath = $"{BTCSettings.TextAddedDir}/{fileName}";
				string screenCapturePath = $"{BTCSettings.OutputDir}/{fileName}";

				using (MagickImage outputImage = new MagickImage(screenCapturePath))
				{
					MagickReadSettings settings = Logic.listOfSettingsForText[i];
					using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))


					{
						// Add the caption layer on top of the background image

						outputImage.Composite(caption, BTCSettings.PositionoftextonHorizontalAxis, BTCSettings.PositionoftextonVerticalAxis, CompositeOperator.Over);
						outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
						outputImage.Quality = 100;
						outputImage.Write(textAddedPath);
					}

				}

			}

		}

		public static void ExtractThumbnails(int i)
		{
			var inputFile = new MediaFile { Filename = BTCSettings.PathToVideo };

			var outputFileName = new MediaFile { Filename = $"{BTCSettings.OutputDir}/{i + 1}.bmp" };

			GrabThumbNail(inputFile, outputFileName, BTCSettings.IntervalBetweenThumbnails);

			BTCSettings.FileNames.Add(Path.GetFileName(outputFileName.Filename));
		}

		// MediaToolKit
		public static void GrabThumbNail(MediaFile inputFile, MediaFile outputFile, int intervalBetweenPictures)
		{
			using (var engine = new Engine())
			{
				engine.GetMetadata(inputFile);

				var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(intervalBetweenPictures), };

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
			// last one is opacity
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
				Width = 1700, // width of text box
			};

			return settingsTextRandom;
		}

		public static void MemeStashDirectories()
		{
			BTCSettings.MemeStashFilePaths = Directory.GetFiles("..\\..\\DankMemeStash");
		}

		public static int SplitMetaDataIntoInterValsForThumbNailCreation()
		{
			double intervalBetweenThumbnailsBased;

			var inputFile = new MediaFile { Filename = BTCSettings.PathToVideo };

			using (var engine = new Engine())
			{
				engine.GetMetadata(inputFile);
				var durationOfMediaFile = inputFile.Metadata.Duration.TotalMinutes;


				intervalBetweenThumbnailsBased = durationOfMediaFile;

			}
			return (int)intervalBetweenThumbnailsBased;
		}

		public static void IncreaseInterval()
		{
			BTCSettings.IntervalBetweenThumbnails += 35;
		}

		public static void TextAdder()
		{
			BTCSettings.ListOfText.Add("The Mandalorian");
		}

	}

}
