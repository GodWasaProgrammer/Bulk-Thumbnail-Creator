using ImageMagick;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
	public class PictureData
		
	{
		/// <summary>
		/// The filename of this object, with relative output path example : output//001.png
		/// </summary>
		private string _FileName;
		public string FileName { get { return _FileName; }set { _FileName = value; } }

		private string _outPath;
		public string OutPath {get { return _outPath; }set { _outPath = value; } }

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

		/// <summary>
		/// Copy Constructor 
		/// </summary>
		/// <param name="pictureDataToCopy"></param>
		public PictureData(PictureData pictureDataToCopy) 
		{
			_FileName = pictureDataToCopy._FileName;
			_outPath = pictureDataToCopy._outPath;
			_ParamForTextCreation = pictureDataToCopy._ParamForTextCreation;
			_ReadSettings = pictureDataToCopy._ReadSettings;

		}
		/// <summary>
		/// Constructor
		/// </summary>
		public PictureData()
		{

		}
	}


}
