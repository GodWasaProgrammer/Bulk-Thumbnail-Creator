using ImageMagick;
using Microsoft.Graph.Reports.GetPrinterArchivedPrintJobsWithPrinterIdWithStartDateTimeWithEndDateTime;
using System.Collections.Generic;

namespace Bulk_Thumbnail_Creator
{
	public class PictureData
	{
		private string _FileName;
		public string FileName { get { return _FileName; }set { _FileName = value; } }

		//private int _IndexOfFile;
		//public int IndexOfFile { get { return _IndexOfFile; } set {_IndexOfFile= value; } }

		private ParamForTextCreation _ParamForTextCreation;
		public ParamForTextCreation ParamForTextCreation { get { return _ParamForTextCreation;} set { _ParamForTextCreation = value; } }

		private MagickReadSettings _ReadSettings;
		public MagickReadSettings ReadSettings { get { return _ReadSettings; } set { _ReadSettings = value; } }


		private List<PictureData> _Varieties = new List<PictureData>();
		public List<PictureData> Varieties { get { return _Varieties; } set { _Varieties = value; } }
	}

}
