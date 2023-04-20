using ImageMagick;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{

	public class BTCSettings
	{
		// directories to create 
		public static string OutputDir { get; set; } = "output";

		public static string TextAddedDir { get; set; } = "text added";

		public static string YoutubeDLDir { get; set; } = "YTDL";

		public static string YoutubeLink { get; set; }

		public static ImageMagick.Gravity PositionOfText = Gravity.Northwest;

		public static int MaxRGB { get; set; } = 256;

		public static List<string> ListOfText { get; set; } = new List<string>();

		public static int NumberOfThumbnails { get; set; } = 25;

		// path to the video to take thumbnails from
		public static string PathToVideo { get; set; }

		// the number of seconds between each thumbnail
		public static int IntervalBetweenThumbnails { get; set; }

		// filenames of our outputted thumbnails
		public static List<string> FileNames { get; set; } = new List<string>();

		// an array of file paths of the dankmemestashfolder
		public static string[] MemeStashFilePaths { get; set; }
	}

}
