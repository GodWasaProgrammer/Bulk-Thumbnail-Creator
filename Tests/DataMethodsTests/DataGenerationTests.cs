using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using DlibDotNet;
using Moq;

namespace Tests.DataMethodsTests
{
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
            int trueCount = 0;
            int falseCount = 0;

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
    }
}
