using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.PictureClasses;
using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.Services;
using BulkThumbnailCreator.Wrappers;
using Moq;

namespace Tests.DataMethodsTests;

public class VarietyTests
{
    [Fact]
    public void Random_Test()
    {
        // Arrange
        var param1 = new ParamForTextCreation { Font = "font1.ttf", Text = "text1" };
        var param2 = new ParamForTextCreation { Font = "font2.ttf", Text = "text2" };

        var logService = new LogService();
        var jobService = new JobService();
        var settings = new Settings(logService, jobService);
        var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();
        mockDirectoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(["font1.ttf", "font2.ttf", "font3.ttf", "font4.ttf", "font5.ttf", "font6.ttf"]);
        var varietyMocked = new Variety(mockDirectoryWrapper.Object, settings);
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
                Assert.True(variety.OutPutType is OutputType.RandomVariety);
            }
            Assert.NotNull(pictureData.Varieties);
            Assert.Equal(5, pictureData.Varieties.Count);
        }
    }

    [Fact]
    public static void Meme_test()
    {
        // Arrange

        // First we make an object to try on
        // and set the fields and list and what not
        var pictureData = new PictureData();
        var outpath = "MemeTest";
        var fileName = "memeTesting";
        pictureData.OutPath = outpath;
        pictureData.FileName = fileName;
        pictureData._numberOfBoxes = 2;
        var copiedPicData = new PictureData(pictureData);
        pictureData.Varieties.Add(copiedPicData);
        var mockSettings = new Mock<ISettings>();
        mockSettings.SetupGet(s => s.Memes).Returns(["meme1", "meme2"]);
        var directoryWrapper = new DirectoryWrapper();
        var varietyInstance = new Variety(directoryWrapper, mockSettings.Object);

        // We make some parameters to pass to the picdata
        var param1 = new ParamForTextCreation { Font = "font1.ttf", Text = "text1" };
        var param2 = new ParamForTextCreation { Font = "font2.ttf", Text = "text2" };
        pictureData.BoxParameters.Add(param1);
        pictureData.BoxParameters.Add(param2);

        // Act
        varietyInstance.Meme(pictureData);

        // Assert

        //We will check so our copy ctor deep copy worked
        Assert.NotSame(pictureData, copiedPicData);

        // We will assert the copy ctor is working as intended
        Assert.Equal(pictureData.FileName, fileName);
        Assert.Equal(outpath, pictureData.OutPath);
        Assert.Equal(2, pictureData._numberOfBoxes);

        // Make sure the Variety itself does not have varieties as it should be clear
        Assert.Empty(pictureData.Varieties[0].Varieties);
        Assert.Empty(pictureData.Varieties[1].Varieties);

        // make sure that we have a Main variety, which is the initial control value added to varietylist
        // and also make sure we have the newly generated memevariety in the varietylist
        Assert.Equal(OutputType.Main, pictureData.Varieties[0].OutPutType);
        Assert.Equal(OutputType.MemeVariety, pictureData.Varieties[1].OutPutType);

        // make sure there is a null meme field in one of the boxes
    }
}
