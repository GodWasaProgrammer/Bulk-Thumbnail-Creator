﻿using ImageMagick;
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
		public static void AddTextComposite(Gravity PositionOfText)
		{
			for (int i = 0; i < BTCSettings.FileNames.Count; i++)
			{
				string fileName = BTCSettings.FileNames[i];

				string textAddedPath = $"{BTCSettings.TextAddedDir}/{fileName}";
				string screenCapturePath = $"{BTCSettings.OutputDir}/{fileName}";

				using (MagickImage outputImage = new MagickImage(screenCapturePath))
				{
					MagickReadSettings settings = Logic.ListOfSettingsForText[i];
					using (var caption = new MagickImage($"caption:{BTCSettings.ListOfText[0]}", settings))

					{
						// Add the caption layer on top of the background image
						// gravity will dictate position of your text instead of x/y by a settings var
						outputImage.Composite(caption,PositionOfText, CompositeOperator.Over);
						outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
						outputImage.Quality = 100;
						// outputs the file to the provided path and name
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
		private static void GrabThumbNail(MediaFile inputFile, MediaFile outputFile, int intervalBetweenPictures)
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
		public static List<MagickReadSettings> ListOfSettingsForText { get; set; } = new List<MagickReadSettings>();

		static readonly Random colorRandom = new Random();

		internal static MagickColor RandomizeColor()
		{
			byte pickedColorRedRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorGreenRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);
			byte pickedColorBlueRGB = (byte)colorRandom.Next(BTCSettings.MaxRGB);

			MagickColor colorRNGPicked;

			colorRNGPicked = MagickColor.FromRgb(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB);

			return colorRNGPicked;
		}

		// attempt at making some sort of semi-linear coloring function
		internal static MagickColor ColorsFallingAndRising()
		{
			
			if (BTCSettings.RisingColorRedRGB != 255)
			{
				BTCSettings.RisingColorRedRGB += 25;

				if (BTCSettings.RisingColorRedRGB == 255)
				{
					BTCSettings.RisingColorRedRGB = 0;
				}
			}

			if (BTCSettings.FallingColorGreenRGB != 0)
			{
				BTCSettings.FallingColorGreenRGB -= 25;

				if (BTCSettings.FallingColorGreenRGB == 0)
				{
					BTCSettings.FallingColorGreenRGB = 255;
				}

			}

			if (BTCSettings.FallingColorBlueRGB != 0)
			{
				BTCSettings.FallingColorBlueRGB -= 25;

				if (BTCSettings.FallingColorBlueRGB == 0)
				{
					BTCSettings.FallingColorBlueRGB = 255;
				}

			}
			MagickColor colorFallingAndRising;
			colorFallingAndRising = MagickColor.FromRgb(BTCSettings.RisingColorRedRGB, BTCSettings.FallingColorGreenRGB, BTCSettings.FallingColorBlueRGB);

			return colorFallingAndRising;
		}

		public static MagickReadSettings GenerateLinearProgressionColorSettings()
		{
			
			MagickReadSettings settingsTextRandom = new MagickReadSettings
			{
				Font = "italic",
				FillColor = ColorsFallingAndRising(),
				StrokeColor = ColorsFallingAndRising(),
				BorderColor = ColorsFallingAndRising(),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 10,
				FontPointsize = 200,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				Width = 1700, // width of text box
			};

			return settingsTextRandom;
		}

		public static MagickReadSettings GenerateRandomColorSettings()
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
				FontPointsize = 175,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				Width = 1700, // width of text box
			};

			return settingsTextRandom;
		}

		public static void MemeStashDirectories()
		{
			BTCSettings.MemeStashFilePaths = Directory.GetFiles("..\\..\\DankMemeStash");
		}

		public static int ReturnTotalDurationOfClip()
		{
			double totalMinutesOfClip;

			var inputFile = new MediaFile { Filename = BTCSettings.PathToVideo };

			using (var engine = new Engine())
			{
				engine.GetMetadata(inputFile);
				totalMinutesOfClip = inputFile.Metadata.Duration.TotalMinutes;
				
			}
			return (int)totalMinutesOfClip;
		}

		public static void IncreaseInterval()
		{
			BTCSettings.IntervalBetweenThumbnails += 5;
		}

		public static void TextAdder(string TextToPrintOnImage)
		{
			BTCSettings.ListOfText.Add(TextToPrintOnImage);
		}

	}

}