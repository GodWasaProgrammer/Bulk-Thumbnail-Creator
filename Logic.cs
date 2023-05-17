using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using UMapx.Imaging;
using UMapx.Colorspace;
using Microsoft.Graph.Models.ExternalConnectors;

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

			TextScheme scheme = new TextScheme();
			scheme.FillColor.SetByHSL(0.50F, 0.35F, 0.25F);
			scheme.StrokeColor.SetByHSL(0.13F, 0.15F, 0.40F);
			scheme.BorderColor.SetByHSL(0.75F, 0.15F, 0.50F);

			MagickReadSettings settingsTextRandom = new MagickReadSettings
			{
				Font = "italic",
				FillColor = MagickColor.FromRgb(scheme.FillColor.Red, scheme.FillColor.Green, scheme.FillColor.Blue),
				StrokeColor = MagickColor.FromRgb(scheme.StrokeColor.Red, scheme.StrokeColor.Green, scheme.StrokeColor.Blue),
				BorderColor = MagickColor.FromRgb(scheme.BorderColor.Red, scheme.BorderColor.Green, scheme.BorderColor.Blue),
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
			TextScheme scheme = new TextScheme();

			MagickReadSettings SettingsTextLinear = new MagickReadSettings
			{
				Font = "linear",
				FillColor = MagickColor.FromRgb(scheme.FillColor.Red, scheme.FillColor.Green, scheme.FillColor.Blue),
				StrokeColor = MagickColor.FromRgb(scheme.FillColor.Red, scheme.FillColor.Green, scheme.FillColor.Blue),
				BorderColor = MagickColor.FromRgb(scheme.FillColor.Red, scheme.FillColor.Green, scheme.FillColor.Blue),
				FontStyle = FontStyleType.Bold,
				StrokeAntiAlias = true,
				StrokeWidth = 6,
				//FontPointsize = 100,
				FontWeight = FontWeight.Bold,
				BackgroundColor = MagickColors.Transparent,
				//Height = 1850, // height of text box
				Width = 1700, // width of text box
			};

			return SettingsTextLinear;
		}

		//public static void LinearColorGeneration()
		//{
		//	TextScheme item = new TextScheme();

		//	int hue = 5;
		//	float saturation = 0.36F;
		//	float lightness = 0.25F;

		//	// item.SetByHSL(hue, saturation, lightness);

		//}

	}

}