namespace BulkThumbnailCreator.PictureClasses;

[Serializable]
public class PictureData
{
    public string FileName { get; set; }
    public string OutPath { get; set; }
    public List<ParamForTextCreation> BoxParameters { get; set; } = [];
    public int _numberOfBoxes = 2;
    public OutputType OutPutType { get; set; }
    [XmlIgnore]
    public MagickReadSettings ReadSettings { get; private set; }
    public MagickReadSettings MakeTextSettings(ParamForTextCreation parameters)
    {
        ReadSettings = TextSettingsGeneration(parameters);
        if (parameters.CurrentBox.Type == BoxType.BottomBox || parameters.CurrentBox.Type == BoxType.BottomLeft || parameters.CurrentBox.Type == BoxType.BottomRight)
        {
            ReadSettings.TextGravity = Gravity.Center;
        }

        return ReadSettings;
    }
    public List<PictureData> Varieties { get; set; } = [];
    /// <summary>
    /// Generates MagickReadSettings to be used in a PicturedataObject to decide how text will look
    /// <param name="parameters">The passed Parameters for text creation</param>
    /// <returns></returns>
    private static MagickReadSettings TextSettingsGeneration(ParamForTextCreation parameters)
    {
        return new()
        {
            Font = parameters.Font,
            FillColor = MagickColor.FromRgb(parameters.FillColor.Red, parameters.FillColor.Green, parameters.FillColor.Blue),
            StrokeColor = MagickColor.FromRgb(parameters.StrokeColor.Red, parameters.StrokeColor.Green, parameters.StrokeColor.Blue),
            StrokeWidth = 5,
            FillRule = FillRule.EvenOdd,
            BackgroundColor = MagickColors.Transparent,
            Height = parameters.HeightOfBox, // height of text box
            Width = parameters.WidthOfBox, // width of text box
            FontStyle = FontStyleType.Bold
        };
    }
    // Copy Constructor 
    public PictureData(PictureData pictureDataToCopy)
    {
        FileName = pictureDataToCopy.FileName;
        BoxParameters = new List<ParamForTextCreation>();
        foreach (var item in pictureDataToCopy.BoxParameters)
        {
            BoxParameters.Add(new ParamForTextCreation(item));
        }
        Varieties = new List<PictureData>();
        foreach (var variety in pictureDataToCopy.Varieties)
        {
            Varieties.Add(new PictureData(variety));
        }
        OutPutType = pictureDataToCopy.OutPutType;
        if (pictureDataToCopy.OutPath != null)
        {
            OutPath = pictureDataToCopy.OutPath;
        }
    }
    public PictureData()
    {
    }
}
