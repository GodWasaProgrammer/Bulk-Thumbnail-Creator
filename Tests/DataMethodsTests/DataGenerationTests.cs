using BulkThumbnailCreator;
using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.Interfaces;
using DlibDotNet;
using Moq;

namespace Tests.DataMethodsTests;

public class DataGenerationTests
{
    [Fact]
    public void PickRandomFont_Test()
    {
        // Arrange
        var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
        mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(new string[] { "font1.ttf", "font2.ttf", "font3.ttf" });

        var fontPicker = new DataGeneration(mockDirectoryWrapper.Object);

        // Act
        var result = fontPicker.PickRandomFont();

        // Assert
        Assert.Contains("font", result); // Validate that the returned font name contains "font"
    }

    [Fact]
    public void RandomGradient_Test()
    {
        // Arrange
        const int iterations = 10000;
        var trueCount = 0;
        var falseCount = 0;

        // Act
        for (var i = 0; i < iterations; i++)
        {
            if (DataGeneration.RandomGradient())
            {
                trueCount++;
            }
            else
            {
                falseCount++;
            }
        }

        // Assert
        Assert.True(trueCount > 0, "RandomGradient should return true at least once.");
        Assert.True(falseCount > 0, "RandomGradient should return false at least once.");

        var trueRatio = (double)trueCount / iterations;
        var falseRatio = (double)falseCount / iterations;

        Assert.InRange(trueRatio, 0.45, 0.55);
        Assert.InRange(falseRatio, 0.45, 0.55);
    }

    [Fact]
    public void IntersectCheck_Test()
    {
        // Arrange
        var rectNoInterSect1 = new Rectangle(50, 50, 100, 100); // Left=50, Top=50, Right=100, Bottom=100
        var rectNoInterSect2 = new Rectangle(150, 150, 200, 200); // Left=150, Top=150, Right=200, Bottom=200
        var RectInterSect1 = new Rectangle(50, 50, 100, 100); // Left=50, Top=50, Right=100, Bottom=100
        var RectInterSect2 = new Rectangle(75, 75, 125, 125); // Left=75, Top=75, Right=125, Bottom=125

        // Act
        var resultShouldReturnFalse = DataGeneration.IntersectCheck(rectNoInterSect1, rectNoInterSect2);
        var resultShouldReturnTrue = DataGeneration.IntersectCheck(RectInterSect1, RectInterSect2);

        // Assert
        Assert.False(resultShouldReturnFalse);
        Assert.True(resultShouldReturnTrue);
    }

    [Fact]
    public void BuildDefaultBoxes_Test()
    {
        // Arrange
        var columns = 200;
        var rows = 100;
        var sourcePicture = new Array2D<RgbPixel>(rows, columns);

        // Act
        var boxes = DataGeneration.BuildDefaultBoxes(sourcePicture);

        // Assert
        Assert.NotNull(boxes);
        Assert.Equal(6, boxes.Count);

        // Verify top box
        var topBox = boxes.FirstOrDefault(b => b.Type == BoxType.TopBox);
        Assert.NotNull(topBox);
        Assert.Equal(columns, topBox.Width);
        Assert.Equal(rows / 2, topBox.Height);
        Assert.Equal(0, topBox.X);
        Assert.Equal(0, topBox.Y);

        // Verify bottom box
        var bottomBox = boxes.FirstOrDefault(b => b.Type == BoxType.BottomBox);
        Assert.NotNull(bottomBox);
        Assert.Equal(columns, bottomBox.Width);
        Assert.Equal(rows / 2, bottomBox.Height);
        Assert.Equal(0, bottomBox.X);
        Assert.Equal(rows / 2, bottomBox.Y);

        // Verify top left box
        var topLeftBox = boxes.FirstOrDefault(b => b.Type == BoxType.TopLeft);
        Assert.NotNull(topLeftBox);
        Assert.Equal(columns / 2, topLeftBox.Width);
        Assert.Equal(rows / 2, topLeftBox.Height);
        Assert.Equal(0, topLeftBox.X);
        Assert.Equal(0, topLeftBox.Y);

        // Verify top right box
        var topRightBox = boxes.FirstOrDefault(b => b.Type == BoxType.TopRight);
        Assert.NotNull(topRightBox);
        Assert.Equal(columns / 2, topRightBox.Width);
        Assert.Equal(rows / 2, topRightBox.Height);
        Assert.Equal(columns / 2, topRightBox.X);
        Assert.Equal(0, topRightBox.Y);

        // Verify bottom left box
        var bottomLeftBox = boxes.FirstOrDefault(b => b.Type == BoxType.BottomLeft);
        Assert.NotNull(bottomLeftBox);
        Assert.Equal(columns / 2, bottomLeftBox.Width);
        Assert.Equal(rows / 2, bottomLeftBox.Height);
        Assert.Equal(0, bottomLeftBox.X);
        Assert.Equal(rows / 2, bottomLeftBox.Y);

        // Verify bottom right box
        var bottomRightBox = boxes.FirstOrDefault(b => b.Type == BoxType.BottomRight);
        Assert.NotNull(bottomRightBox);
        Assert.Equal(columns / 2, bottomRightBox.Width);
        Assert.Equal(rows / 2, bottomRightBox.Height);
        Assert.Equal(columns / 2, bottomRightBox.X);
        Assert.Equal(rows / 2, bottomRightBox.Y);
    }

    [Fact]
    public void ExcludePopulatedBoxesIntersect_Test()
    {
        // Arrange
        var rows = 100;
        var columns = 200;
        var srcPic = new Array2D<RgbPixel>(rows, columns);
        var defaultBoxes = DataGeneration.BuildDefaultBoxes(srcPic);

        // make dummy populatedbox list
        var populatedBoxes = new List<BoxType> { BoxType.BottomBox };

        // make a dummy freebox list
        var freeBoxes = new List<BoxType> { BoxType.BottomRight, BoxType.BottomBox, BoxType.TopBox, BoxType.TopLeft, BoxType.TopRight };

        // Act
        DataGeneration.ExcludePopulatedBoxesIntersection(populatedBoxes, defaultBoxes, freeBoxes);

        // Assert
        Assert.DoesNotContain(BoxType.BottomBox, freeBoxes); // BottomBox should be removed from freeBoxes
        Assert.Contains(BoxType.TopBox, freeBoxes); // TopBox should remain in freeBoxes
        Assert.Contains(BoxType.TopLeft, freeBoxes);
        Assert.Contains(BoxType.TopRight, freeBoxes);
        Assert.Equal(3, freeBoxes.Count);
    }
}
