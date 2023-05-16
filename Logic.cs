using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using UMapx.Imaging;
using UMapx.Colorspace;

namespace Bulk_Thumbnail_Creator
{

	internal class Logic
	{

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

		public static MagickReadSettings AdaptableSizeText()
		{
			int FontPointSize = 75;
			string path = $"{Path.GetFullPath(BTCSettings.OutputDir)}/001.png";
			Image img = Image.FromFile(path);

			if (img.Width == 1280 || img.Height == 720)
			{
				FontPointSize = 100;
			}

			if (img.Width == 1920 || img.Height == 1080)
			{
				FontPointSize = 175;
			}

			if (img.Width > 1920)
			{
				FontPointSize = 275;
			}

			if (img.Width < 1280)
			{
				FontPointSize = 75;
			}

			MagickReadSettings settingsTextRandom = new MagickReadSettings
			{
				Font = "italic",
				FillColor = MagickColor.FromRgb(210, 255, 0),
				StrokeColor = MagickColor.FromRgb(255, 45, 0),
				BorderColor = MagickColor.FromRgb(38, 0, 255),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 4,
				FontWeight = FontWeight.Bold,
				FontPointsize = FontPointSize,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				//Width = 1700, // width of text box
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
				StrokeWidth = 6,
				//FontPointsize = 100,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				Width = 1700, // width of text box
			};

			return settingsTextRandom;
		}

		public static MagickReadSettings Linear()
		{
			MagickReadSettings SettingsTextLinear = new MagickReadSettings
			{
				Font = "linear",
			};

			return SettingsTextLinear;
		}

		public static void LinearColorGeneration()
		{
			ColorItem item = new ColorItem();

			int hue = 5;
			float saturation = 0.36F;
			float lightness = 0.25F;

			item.SetByHSL(hue, saturation, lightness);

		}

	}

}