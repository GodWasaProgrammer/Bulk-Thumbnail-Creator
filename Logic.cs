using ImageMagick;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bulk_Thumbnail_Creator
{

	internal class Logic
	{
		public static void AddTextComposite(Gravity PositionOfText)
		{
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
						outputImage.Composite(caption,PositionOfText, CompositeOperator.Over);
						outputImage.Annotate("Bulk Thumbnail Creator", gravity: Gravity.North);
						outputImage.Quality = 100;
						// outputs the file to the provided path and name
						outputImage.Write(textAddedOutPutFile);

					}

				}

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

		public static void TextAdder(string TextToPrintOnImage)
		{
			BTCSettings.ListOfText.Add(TextToPrintOnImage);
		}

	}

}