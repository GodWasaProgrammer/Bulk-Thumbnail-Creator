using Bulk_Thumbnail_Creator.Enums;
using System.Drawing;

namespace Bulk_Thumbnail_Creator.PictureObjects
{
    public class Box
    {
        public Box()
        {

        }

        public Box(Box currentBox)
        {
            X = currentBox.X; // int
            Y = currentBox.Y; // int
            Width = currentBox.Width; // int
            Height = currentBox.Height; // int
            Type = currentBox.Type; // BoxType
            Rectangle = currentBox.Rectangle; // Rectangle
        }
        private int _X; 
        public int X { get { return _X; } set { _X = value; } }

        
        private int _Y; 
        public int Y { get { return _Y; } set { _Y = value; } }

        
        private int _Width; 
        public int Width { get { return _Width; } set { _Width = value; } }

        
        private int _Height; 
        public int Height { get { return _Height; } set { _Height = value; } }

        
        private BoxType _type; 
        public BoxType Type { get { return _type; } set { _type = value; } }


        private Rectangle _rectangle;

        public Rectangle Rectangle { get { return _rectangle;  } set { _rectangle = value; } }

    }

}
