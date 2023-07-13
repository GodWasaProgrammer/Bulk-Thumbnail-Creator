using System.Collections.Generic;
using System.Drawing;

namespace Bulk_Thumbnail_Creator
{
	public class ParamForTextCreation
	{
		/// <summary>
		/// Text that will be printed on the image for this object
		/// </summary>
		private string _Text;
		public string Text { get { return _Text; } set { _Text = value; } }

		/// <summary>
		/// The Location of the text that will be printed, this a x,y coordinate pointer
		/// </summary>
		private Point positionOfText;
		public Point PositionOfText { get {  return positionOfText; } set {  positionOfText = value; } }

		/// <summary>
		/// The selected font of this object
		/// </summary>
		private string _Font;
		public string Font { get { return _Font; }set { _Font = value; } }

		/// <summary>
		/// The fontpointsize for this object, 70 is just default, this will always be overwritten
		/// </summary>
		private int _FontPointSize = 70;
		public  int FontPointSize { get { return _FontPointSize; } set { _FontPointSize = value; } }

		/// <summary>
		/// The Box Width which in text will be placed of this object
		/// </summary>
		private int _WidthOfBox;
		public int WidthOfBox { get { return _WidthOfBox; } set { _WidthOfBox = value; } }

		private int _HeightOfBox;
		public int HeightOfBox { get { return _HeightOfBox; } set { _HeightOfBox = value; } }

		/// <summary>
		/// The Fillcolor for textoutput
		/// </summary>
		private ColorItem fillColor = new ColorItem();
		public ColorItem FillColor { get { return fillColor; } set { fillColor = FillColor; } }

		/// <summary>
		/// StrokeColor for textoutput
		/// </summary>
		private ColorItem strokeColor = new ColorItem();
		public ColorItem StrokeColor { get { return strokeColor; } set { strokeColor = StrokeColor; } }

		/// <summary>
		/// bordercolor for textoutput
		/// </summary>
		private ColorItem borderColor = new ColorItem();
		public ColorItem BorderColor { get { return borderColor; } set { borderColor = BorderColor; } }
		

		private Dictionary<Box,Rectangle> _Boxes = new Dictionary<Box,Rectangle>();
		// store possible boxes
		public Dictionary<Box, Rectangle> Boxes { get { return _Boxes; } set { _Boxes = value; } }


		private Box _CurrentBox;
		public Box CurrentBox { get { return _CurrentBox; } set { _CurrentBox = value; } }


		public ParamForTextCreation(ParamForTextCreation paramForTextCreationToCopy)
		{
			_Text = paramForTextCreationToCopy.Text;
			positionOfText = paramForTextCreationToCopy.PositionOfText;
			// _Font = paramForTextCreationToCopy.Font;
			_WidthOfBox = paramForTextCreationToCopy.WidthOfBox;
			_HeightOfBox = paramForTextCreationToCopy.HeightOfBox;
			fillColor = paramForTextCreationToCopy.FillColor;
			strokeColor = paramForTextCreationToCopy.StrokeColor;
			borderColor = paramForTextCreationToCopy.BorderColor;
			_Boxes = paramForTextCreationToCopy._Boxes;
			_CurrentBox = paramForTextCreationToCopy._CurrentBox;
		}
		public ParamForTextCreation()
		{

		}
	}

}
