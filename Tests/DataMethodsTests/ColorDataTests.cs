using BulkThumbnailCreator;
using BulkThumbnailCreator.PictureClasses;

namespace Tests.DataMethodsTests;

public class ColorDataTests
{
    [Fact]
    public void SelectTwoRandomColors_But_Different_SetsFillColorAndStrokeColor()
    {
        // Arrange
        var paramIn = new ParamForTextCreation();

        // Act
        ColorData.SelectTwoRandomColors(paramIn);

        // Assert
        Assert.NotNull(paramIn.FillColor);
        Assert.NotNull(paramIn.StrokeColor);
        Assert.NotEqual(paramIn.FillColor, paramIn.StrokeColor);
    }

    [Fact]
    public void SelectTwoDifferentColors_SetsFillColorAndStrokeColor()
    {
        // Arrange
        var param = new ParamForTextCreation();

        // Act
        ColorData.SelectTwoDifferentColors(param);

        // Assert
        Assert.NotNull(param.FillColor);
        Assert.NotNull(param.StrokeColor);
        Assert.NotEqual(param.FillColor.Red, param.StrokeColor.Red);
        Assert.NotEqual(param.FillColor.Green, param.StrokeColor.Green);
        Assert.NotEqual(param.FillColor.Blue, param.StrokeColor.Blue);
    }

    [Fact]
    public void MakeQuantumColors_ConvertsToMagickColorFromColorItem_RGB()
    {
        // Arrange
        var black = new ColorItem();
        black.SetByRGB(0, 0, 0);
        var white = new ColorItem();
        white.SetByRGB(255, 255, 255);

        // Act
        var resultBlack = ColorData.MakeQuantumColor(black);
        var resultWhite = ColorData.MakeQuantumColor(white);

        //Assert

        // test Black Conversion
        Assert.Equal((ushort)0, resultBlack.R);
        Assert.Equal((ushort)0, resultBlack.G);
        Assert.Equal((ushort)0, resultBlack.B);

        // test White Conversion
        Assert.Equal((ushort)65535, resultWhite.R);
        Assert.Equal((ushort)65535, resultWhite.G);
        Assert.Equal((ushort)65535, resultWhite.B);
    }
}

