using BulkThumbnailCreator.Enums;
using ImageMagick;
using System.Xml.Serialization;

namespace BulkThumbnailCreator.PictureObjects;

[Serializable]
public class PictureData
{
    /// <summary>
    /// The filename of this object, with relative output path example : output//001.png
    /// </summary>
    private string _FileName;
    public string FileName { get { return _FileName; } set { _FileName = value; } }

    private string _outPath;
    public string OutPath { get { return _outPath; } set { _outPath = value; } }

    private List<Box> _boxes = [];
    public List<Box> Boxes { get { return _boxes; } set { _boxes = value; } }

    private List<ParamForTextCreation> _BoxParameters = new();
    public List<ParamForTextCreation> BoxParameters { get { return _BoxParameters; } set { _BoxParameters = value; } }

    public int NumberOfBoxes = 2;

    /// <summary>
    /// ImageMagick settings that belongs to this object, this is used to generate color/textsettings
    /// </summary>
    /// 
    [XmlIgnore]
    public MagickReadSettings ReadSettings { get; private set; }

    public void MakeTextSettings(ParamForTextCreation Parameters)
    {
        ReadSettings = TextSettingsGeneration(Parameters);
    }

    /// <summary>
    /// List of varieties belonging to this image
    /// </summary>
    /// 
    private List<PictureData> _Varieties = [];
    public List<PictureData> Varieties { get { return _Varieties; } set { _Varieties = value; } }

    /// <summary>
    /// Copy Constructor 
    /// </summary>
    /// <param name="pictureDataToCopy"></param>
    public PictureData(PictureData pictureDataToCopy)
    {
        _FileName = (string)pictureDataToCopy.FileName.Clone();
        BoxParameters = [];
        foreach (var item in pictureDataToCopy.BoxParameters)
        {
            BoxParameters.Add(new ParamForTextCreation(item));
        }
        _Varieties = new List<PictureData>(pictureDataToCopy.Varieties);
        _OutputType = pictureDataToCopy.OutPutType;
        if (pictureDataToCopy.OutPath != null)
        {
            _outPath = (string)pictureDataToCopy.OutPath.Clone();
        }
    }

    public PictureData()
    {

    }

    private OutputType _OutputType;
    public OutputType OutPutType { get { return _OutputType; } set { _OutputType = value; } }


    /// <summary>
    /// Generates MagickReadSettings to be used in a PicturedataObject to decide how text will look
    /// </summary>
    /// <param name="Parameters">The passed Parameters for text creation</param>
    /// <returns></returns>
    private static MagickReadSettings TextSettingsGeneration(ParamForTextCreation Parameters)
    {
        MagickReadSettings SettingsTextLinear = new()
        {
            Font = Parameters.Font,
            FillColor = MagickColor.FromRgb(Parameters.FillColor.Red, Parameters.FillColor.Green, Parameters.FillColor.Blue),
            StrokeColor = MagickColor.FromRgb(Parameters.StrokeColor.Red, Parameters.StrokeColor.Green, Parameters.StrokeColor.Blue),
            BorderColor = MagickColor.FromRgb(Parameters.BorderColor.Red, Parameters.BorderColor.Green, Parameters.BorderColor.Blue),
            StrokeWidth = 6,
            BackgroundColor = MagickColors.Transparent,
            Height = Parameters.HeightOfBox, // height of text box
            Width = Parameters.WidthOfBox, // width of text box
            FontStyle = FontStyleType.Any,
        };

        return SettingsTextLinear;
    }
}