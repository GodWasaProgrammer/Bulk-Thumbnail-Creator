namespace BulkThumbnailCreator.PictureClasses;

public class ParamForTextCreation
{
    public string Meme { get; set; }
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
        if (Text is not null)
        {
            Text = (string)param.Text.Clone(); // string
        }
        if (Font is not null)
        {
            Font = (string)param.Font.Clone(); // string
        }
        WidthOfBox = param.WidthOfBox; // int
        HeightOfBox = param.HeightOfBox; // int
        Meme = (string)param.Meme?.Clone(); // string
        FillColor = new ColorItem(param.FillColor); // object
        StrokeColor = new ColorItem(param.StrokeColor); // object
        Boxes = new List<Box>(param.Boxes); // dictionary
        CurrentBox = new(param.CurrentBox); // object
    }

    public ParamForTextCreation()
    {
    }
}
