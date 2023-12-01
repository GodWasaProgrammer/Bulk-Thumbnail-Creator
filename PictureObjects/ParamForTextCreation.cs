using Bulk_Thumbnail_Creator.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace Bulk_Thumbnail_Creator.PictureObjects
{
    public class ParamForTextCreation
    {
        private string _Meme;
        public string Meme { get { return _Meme; } set { _Meme = value; } }

        /// <summary>
        /// Text that will be printed on the image for this object
        /// </summary>
        private string _Text;
        public string Text { get { return _Text; } set { _Text = value; } }

        /// <summary>
        /// The Location of the text that will be printed, this a x,y coordinate pointer
        /// </summary>
        private Point positionOfText;
        public Point PositionOfText { get { return positionOfText; } set { positionOfText = value; } }

        /// <summary>
        /// The selected font of this object
        /// </summary>
        private string _Font;
        public string Font { get { return _Font; } set { _Font = value; } }


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
        private ColorItem fillColor = new();
        public ColorItem FillColor { get { return fillColor; } set { fillColor = FillColor; } }

        /// <summary>
        /// StrokeColor for textoutput
        /// </summary>
        private ColorItem strokeColor = new();
        public ColorItem StrokeColor { get { return strokeColor; } set { strokeColor = StrokeColor; } }

        /// <summary>
        /// bordercolor for textoutput
        /// </summary>
        private ColorItem borderColor = new();
        public ColorItem BorderColor { get { return borderColor; } set { borderColor = BorderColor; } }

        //[XmlIgnore]
        //private Dictionary<BoxType, Rectangle> _Boxes = new();

        private List<BoxType> _boxesWithNoFaceIntersect = new();
        public List<BoxType> BoxesWithNoFaceIntersect { get { return _boxesWithNoFaceIntersect; } set { _boxesWithNoFaceIntersect = value; } }
        
        private List<Box> _Boxes = new();
        public List<Box> Boxes { get { return _Boxes; } set { _Boxes = value; } }

        private Box _CurrentBox = new();
        public Box CurrentBox { get { return _CurrentBox; } set { _CurrentBox = value; } }


        public ParamForTextCreation(ParamForTextCreation param)
        {
            _Text = (string)param.Text.Clone(); // string
            positionOfText = param.PositionOfText; // struct
            _Font = (string)param.Font.Clone(); // string
            _WidthOfBox = param.WidthOfBox; // int
            _HeightOfBox = param.HeightOfBox; // int
            fillColor = new ColorItem(param.FillColor); // object
            strokeColor = new ColorItem(param.StrokeColor); // object
            borderColor = new ColorItem(param.BorderColor); // object
            Boxes = new List<Box>(param.Boxes); // dictionary
            _CurrentBox = new(param._CurrentBox); // object
        }

        public ParamForTextCreation()
        {

        }

    }

}