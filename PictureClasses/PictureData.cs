// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        FileName = (string)pictureDataToCopy.FileName.Clone();
        BoxParameters = [];
        foreach (var item in pictureDataToCopy.BoxParameters)
        {
            BoxParameters.Add(new ParamForTextCreation(item));
        }
        Varieties = new List<PictureData>(pictureDataToCopy.Varieties);
        OutPutType = pictureDataToCopy.OutPutType;
        if (pictureDataToCopy.OutPath != null)
        {
            OutPath = (string)pictureDataToCopy.OutPath.Clone();
        }
    }
    public PictureData()
    {
    }
}
