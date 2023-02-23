using ImageMagick;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bulk_Thumbnail_Creator
{
	public class BTCSettings
	{
		// directories to create 
		public static string outputDir = "output";
		public static string textAddedDir = "text added";

		// path to the video to take thumbnails from
		public static string pathToVideo = "..\\..\\testvideo.mp4";

		// the number of seconds between each thumbnail
		public static int intervalBetweenThumbnails = 15;

		// paths to our outputted thumbnails
		public static List<string> FilePaths = new List<string>();

		// list of created Presets for printing text to image
		public static List<MagickReadSettings> listOfSettingsForText = new List<MagickReadSettings>();

		XmlSerializer serializer = new XmlSerializer(typeof(MagickReadSettings));

		public static void IncreaseInterval()
		{
			intervalBetweenThumbnails += 15;
		}

		public static void AddSettings()
		{
			MagickReadSettings settingsTextOne = new MagickReadSettings
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

		}
	}

	
}
