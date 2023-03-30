using System.IO;

namespace Bulk_Thumbnail_Creator
{
	public class Program
	{
		public static void Main()
		{
			// creates our 2 dirs to push out unedited thumbnails, and the edited thumbnails.
			Directory.CreateDirectory(BTCSettings.OutputDir);
			Directory.CreateDirectory(BTCSettings.TextAddedDir);

			// creates x images from a mediafile
			for (int i = 0; i < BTCSettings.NumberOfThumbnails; i++)
			{
				Logic.ExtractThumbnails(i);
			}

			// loops foreach file in list of filepaths, generate some settings, return the settings, add em to our listofsettingsfortext.
			foreach (string fileName in BTCSettings.FileNames)
			{
				Logic.listOfSettingsForText.Add(Logic.GenerateColorSettings());
			}

			/// dev area
			Logic.TextAdder();
			Logic.MemeStashDirectories();

			//
			Logic.AddTextComposite();
		}

	}

}