using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.PictureClasses;
using Moq;

namespace Tests.DataMethodsTests;

public class VarietyTests
{
    [Fact]
    public void Variety_Tests()
    {
        // Arrange
        var param1 = new ParamForTextCreation { Font = "font1.ttf", Text = "text1" };
        var param2 = new ParamForTextCreation { Font = "font2.ttf", Text = "text2" };
        var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
        mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(new string[] { "font1.ttf", "font2.ttf", "font3.ttf", "font4.ttf", "font5.ttf", "font6.ttf" });
        var varietyMocked = new Variety(mockDirectoryWrapper.Object);
        var picdata1 = new PictureData
        {
            FileName = "VarietyTest",
            _numberOfBoxes = 2,
            Varieties = []
        };
        picdata1.BoxParameters.Add(param1);
        picdata1.BoxParameters.Add(param2);

        var picdata2 = new PictureData
        {
            FileName = "VarietyTest",
            _numberOfBoxes = 2,
            Varieties = []
        };
        picdata2.BoxParameters.Add(param1);
        picdata2.BoxParameters.Add(param2);

        var picdata3 = new PictureData
        {
            FileName = "VarietyTest",
            _numberOfBoxes = 2,
            Varieties = []
        };
        picdata3.BoxParameters.Add(param1);
        picdata3.BoxParameters.Add(param2);
        var picdata4 = new PictureData
        {
            FileName = "VarietyTest",
            _numberOfBoxes = 2,
            Varieties = []
        };
        picdata4.BoxParameters.Add(param1);
        picdata4.BoxParameters.Add(param2);

        var pictureDatas = new List<PictureData> { picdata1, picdata2, picdata3, picdata4 };

        // Act

        // call the varietymethod on all the picdatas
        foreach (var pictureData in pictureDatas)
        {
            varietyMocked.Random(pictureData);
        }

        // Assert
        foreach (var pictureData in pictureDatas)
        {
            foreach (var variety in pictureData.Varieties)
            {
                var pickedFonts = new List<string>();
                foreach (var boxparameter in variety.BoxParameters)
                {
                    pickedFonts.Add(boxparameter.Font);
                }
                Assert.NotNull(pickedFonts[0]);
                Assert.NotNull(pickedFonts[1]);
                Assert.NotEqual(pickedFonts[0], pickedFonts[1]);
            }
        }
    }
}
