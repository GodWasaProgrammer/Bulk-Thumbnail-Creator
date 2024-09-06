namespace BulkThumbnailCreator.PictureClasses;

public class ParamForTextCreation
{
    public string Meme { get; set; }
    public bool MemeBool { get; set; }
    public bool Shadows { get; set; }
    public string Text { get; set; }
    public string Font { get; set; }
    public bool Gradient { get; set; }
    public int WidthOfBox { get; set; }
    public int HeightOfBox { get; set; }
    public ColorItem FillColor { get; set; } = new();
    public ColorItem StrokeColor { get; set; } = new();
    public List<BoxType> BoxesWithNoFaceIntersect { get; set; } = [];
    public List<Box> Boxes { get; set; } = [];
    public Box CurrentBox { get; set; } = new();
    public ParamForTextCreation(ParamForTextCreation param)
    {
        Text = param.Text; // string
        Font = param.Font; // string
        WidthOfBox = param.WidthOfBox; // int
        HeightOfBox = param.HeightOfBox; // int
        Meme = param.Meme; // string
        MemeBool = param.MemeBool; // boolean
        Shadows = param.Shadows; // boolean
        FillColor = new ColorItem(param.FillColor); // object
        StrokeColor = new ColorItem(param.StrokeColor); // object
        Boxes = new List<Box>();
        foreach (var box in param.Boxes)
        {
            Boxes.Add(new Box(box)); // Förutsätter att Box har en copy-konstruktor
        }
        CurrentBox = new(param.CurrentBox); // object
    }

    public ParamForTextCreation()
    {
    }
}
