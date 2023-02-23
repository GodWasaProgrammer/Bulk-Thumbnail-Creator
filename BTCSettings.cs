using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Bulk_Thumbnail_Creator
{

	public class BTCSettings
	{
		// directories to create 
		public static string outputDir = "output";
		public static string textAddedDir = "text added";
		public static int MaxRGB = 256;

		// horizontal positioning of text composition
		public static int positionoftextonHorizontalAxis = 0;

		// vertical position of text composition
		public static int positionoftextonVerticalAxis = 0;

		public static int LowerPositionHorizontalAxis = 850;

		const string PATH = "TextPrintSettings.xml";

		// path to the video to take thumbnails from
		public static string pathToVideo = "..\\..\\testvideo.mp4";

		// the number of seconds between each thumbnail
		public static int intervalBetweenThumbnails = 15;

		// paths to our outputted thumbnails
		public static List<string> FilePaths = new List<string>();

		// list of created Presets for printing text to image
		public static List<MagickReadSettings> listOfSettingsForText = new List<MagickReadSettings>();

		// static XmlSerializer serializer = new XmlSerializer(typeof(MagickReadSettings));

		static Random colorRandom = new Random();

		public static void IncreaseInterval()
		{
			intervalBetweenThumbnails += 15;
		}

		internal static MagickColor RandomizeColor()
		{
			byte pickedColorRedRGB = (byte)colorRandom.Next(MaxRGB);
			byte pickedColorGreenRGB = (byte)colorRandom.Next(MaxRGB);
			byte pickedColorBlueRGB = (byte)colorRandom.Next(MaxRGB);
			// byte pickedColorAlphaRGB = (byte)colorRandom.Next(MaxRGB);

			MagickColor colorRNGPicked = new MagickColor();

			colorRNGPicked = MagickColor.FromRgba(pickedColorRedRGB, pickedColorGreenRGB, pickedColorBlueRGB, 255);

			return colorRNGPicked;
		}
		public static void RandomizeTextColorSettings()
		{
			foreach (string filepath in FilePaths)
			{
				MagickReadSettings settingsTextRandom = new MagickReadSettings
				{
					Font = "italic",
					FillColor = RandomizeColor(),
					StrokeColor = RandomizeColor(),
					BorderColor = RandomizeColor(),
					FontStyle = FontStyleType.Bold,
					FontPointsize = 200,
					FontWeight = FontWeight.Bold,
					BackgroundColor = MagickColors.Transparent,
					Height = 1850, // height of text box
					Width = 1900, // width of text box

				};

				listOfSettingsForText.Add(settingsTextRandom);
			}

		}

	}

}
