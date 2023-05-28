namespace Bulk_Thumbnail_Creator
{
	public class PictureData
	{
		private string _name;
		public string FileName { get { return _name; }set { _name = value; } }

		private int _IndexOfFile;
		public int IndexOfFile { get { return _IndexOfFile; } set {_IndexOfFile= value; } }

		private ParamForTextCreation _ParamForTextCreation;
		public ParamForTextCreation ParamForTextCreation { get { return _ParamForTextCreation;} set { _ParamForTextCreation= value; } }

	}

}
