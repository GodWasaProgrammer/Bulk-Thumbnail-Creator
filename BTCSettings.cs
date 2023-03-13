using System.Collections.Generic;
using System.IO;

namespace Bulk_Thumbnail_Creator
{

	public class BTCSettings
	{
		// directories to create 
		public static string outputDir = "output";

		public static string textAddedDir = "text added";

		public static int MaxRGB = 256;

		public static List<string> listOfText= new List<string>();

		public static int numberOfThumbnails = 25;

		// horizontal positioning of text composition
		public static int positionoftextonHorizontalAxis = 0;

		// vertical position of text composition
		public static int positionoftextonVerticalAxis = 0;

		public static int LowerPositionHorizontalAxis = 850;

		// path to the video to take thumbnails from
		public static string pathToVideo = "..\\..\\testvideo.mp4";

		// the number of seconds between each thumbnail
		public static int intervalBetweenThumbnails = 15;

		// paths to our outputted thumbnails
		public static List<string> FilePaths = new List<string>();

		// an array of file paths of the dankmemestashfolder
		public static string[] memeStashFilePaths;

		public static void IncreaseInterval()
		{
			intervalBetweenThumbnails += 5;
		}

		public static void TextAdderTemp()
		{
			listOfText.Add("BTC, Your friendly Thumbnail Creator");
			listOfText.Add("Check it out!");
		}
	}

}
