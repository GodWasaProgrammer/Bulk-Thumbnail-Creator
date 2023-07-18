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
		public MagickReadSettings ReadSettings { get { return _ReadSettings = Logic.TextSettingsGeneration(ParamForTextCreation); } }

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
			_FileName = (string)pictureDataToCopy.FileName.Clone();
			_ParamForTextCreation = new ParamForTextCreation(pictureDataToCopy.ParamForTextCreation);
			_Varieties = new List<PictureData>();
			if (_outPath != null)
			{
                _outPath = (string)pictureDataToCopy.OutPath.Clone();
            }
			_DankBox = pictureDataToCopy.Dankbox;
			if (_Meme != null)
			{
                _Meme = (string)pictureDataToCopy._Meme.Clone();
            }
			
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public PictureData() 
		{ 

		}

		private Box _DankBox;
		public Box Dankbox {get { return _DankBox; } set { _DankBox = value; } }


		private string _Meme;
		public string Meme {get { return _Meme; }set { _Meme = value; } }


		private OutputType _OutputType;
		public OutputType OutputType { get { return _OutputType; } set { _OutputType = value; } }
	}


}
