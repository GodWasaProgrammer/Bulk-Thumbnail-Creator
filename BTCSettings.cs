using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{

	public class BTCSettings
	{
		// directories to create 
		public static string OutputDir { get; set; } = "output";

		public static string TextAddedDir = "text added";

		public static string YoutubeDLDir = "YTDL";

		public static string YoutubeLink;

		public static int MaxRGB = 256;

		public static List<string> ListOfText = new List<string>();

		public static int NumberOfThumbnails { get; set; } = 25;

		// horizontal positioning of text composition
		public static int PositionoftextonHorizontalAxis = 10;

		// vertical position of text composition
		public static int PositionoftextonVerticalAxis = 600;

		public static int LowerPositionHorizontalAxis = 850;

		// path to the video to take thumbnails from
		public static string PathToVideo;

		// the number of seconds between each thumbnail
		public static int IntervalBetweenThumbnails;

		// filenames of our outputted thumbnails
		public static List<string> FileNames { get; set; } = new List<string>();

		// an array of file paths of the dankmemestashfolder
		public static string[] MemeStashFilePaths;
	}

}
