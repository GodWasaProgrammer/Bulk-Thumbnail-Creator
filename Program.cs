using System.IO;
using Bulk_Thumbnail_Creator;

namespace BTC_Prototype
{
	public class Program
	{
		public static void Main()
		{
			// creates our 2 dirs to push out unedited thumbnails, and the edited thumbnails.
			Directory.CreateDirectory(BTCSettings.outputDir);
			Directory.CreateDirectory(BTCSettings.textAddedDir);

			// creates x images from a mediafile
			for (int i = 0; i < BTCSettings.numberOfThumbnails; i++)
			{
				Logic.ExtractThumbnails(i);
			}

			// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
			foreach (string filepath in BTCSettings.FilePaths)
			{
				Logic.listOfSettingsForText.Add(Logic.GenerateColorSettings());
			}

			/// dev area
			BTCSettings.TextAdderTemp();
			Logic.MemeStashDirectories();

			//
			Logic.AddTextComposite();
		}

	}

}