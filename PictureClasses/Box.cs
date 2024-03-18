namespace BulkThumbnailCreator.PictureClasses;

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
    }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public BoxType Type { get; set; }
    public Rectangle Rectangle { get; set; }
}
