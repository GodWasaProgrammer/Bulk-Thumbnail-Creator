using ImageMagick;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
	public class PictureData
	{
		private string _FileName;
		public string FileName { get { return _FileName; }set { _FileName = value; } }

		private int _IndexOfFile;
		public int IndexOfFile { get { return _IndexOfFile; } set {_IndexOfFile= value; } }

		private ParamForTextCreation _ParamForTextCreation;
		public ParamForTextCreation ParamForTextCreation { get { return _ParamForTextCreation;} set { _ParamForTextCreation= value; } }

		public MagickReadSettings ReadSettings { get; set; }

		public List<PictureData> Varieties { get; set; }

	}

}
