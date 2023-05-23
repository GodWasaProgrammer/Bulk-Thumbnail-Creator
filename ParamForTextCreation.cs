using System.Drawing;

namespace Bulk_Thumbnail_Creator
{
	public class ParamForTextCreation
	{
		private Point positionOfText;
		public Point PositionOfText { get {  return positionOfText; } set {  positionOfText = value; } }


		private int _FontPointSize = 70;
		public  int FontPointSize { get { return _FontPointSize; } set { _FontPointSize = value; } }


		ColorItem fillColor = new ColorItem();
		public ColorItem FillColor { get { return fillColor; } set { fillColor = FillColor; } }


		ColorItem strokeColor = new ColorItem();
		public ColorItem StrokeColor { get { return strokeColor; } set { strokeColor = StrokeColor; } }


		ColorItem borderColor = new ColorItem();
		public ColorItem BorderColor { get { return borderColor; } set { borderColor = BorderColor; } }
	}

}
