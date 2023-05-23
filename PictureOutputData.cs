namespace Bulk_Thumbnail_Creator
{
	internal class PictureOutputData
	{
		private string _name;
		public string FileName { get { return _name; }set { _name = value; } }

		private ParamForTextCreation _ParamForTextCreation;
		public ParamForTextCreation ParamForTextCreation { get { return _ParamForTextCreation;} set { _ParamForTextCreation= value; } }


		public PictureOutputData(string name, string fileName, ParamForTextCreation paramForTextCreation, ParamForTextCreation ParamForTextCreation)
		{
			_name = name;
			FileName = fileName;
			ParamForTextCreation = _ParamForTextCreation;
		}

	}

}
