using ImageMagick;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
	public class PictureData
	{
		/// <summary>
		/// The filename of this object, not a path, example : 001.png
		/// </summary>
		private string _FileName;
		public string FileName { get { return _FileName; }set { _FileName = value; } }

		/// <summary>
		/// Parameters for text creation, contains all necessary info like font,position,colors
		/// </summary>
		private ParamForTextCreation _ParamForTextCreation;
		public ParamForTextCreation ParamForTextCreation { get { return _ParamForTextCreation;} set { _ParamForTextCreation = value; } }

		/// <summary>
		/// ImageMagick settings that belongs to this object, this is used to generate color/textsettings
		/// </summary>
		private MagickReadSettings _ReadSettings;
		public MagickReadSettings ReadSettings { get { return _ReadSettings; } set { _ReadSettings = value; } }

		/// <summary>
		/// List of varieties belonging to this image
		/// </summary>
		private List<PictureData> _Varieties = new List<PictureData>();
		public List<PictureData> Varieties { get { return _Varieties; } set { _Varieties = value; } }

		public BTCSettings.BoxTypes Boxes;

	}

}
