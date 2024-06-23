using BulkThumbnailCreator;
using BulkThumbnailCreator.PictureClasses;


namespace Tests
{
    public class ColorDataTests
    {
        [Fact]
        public void SelectTwoRandomColors_SetsFillColorAndStrokeColor()
        {
            // Arrange
            var paramIn = new ParamForTextCreation();

            // Act
            var result = ColorData.SelectTwoRandomColors(paramIn);

            // Assert
            Assert.NotNull(result.FillColor);
            Assert.NotNull(result.StrokeColor);
            Assert.NotEqual(result.StrokeColor.Red, result.FillColor.Red);
            Assert.NotEqual(result.StrokeColor.Green, result.FillColor.Green);
            Assert.NotEqual(result.StrokeColor.Blue, result.FillColor.Blue);
        }
        [Fact]
        public void SelectTwoDifferentColors_SetsFillColorAndStrokeColor()
        {
            // Arrange
            var param = new ParamForTextCreation();

            // Act
            var result = ColorData.SelectTwoDifferentColors(param);

            // Assert
            Assert.NotNull(result.FillColor);
            Assert.NotNull(result.StrokeColor);
            Assert.NotEqual(result.FillColor.Red, result.StrokeColor.Red);
            Assert.NotEqual(result.FillColor.Green, result.StrokeColor.Green);
            Assert.NotEqual(result.FillColor.Blue, result.StrokeColor.Blue);
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
}

