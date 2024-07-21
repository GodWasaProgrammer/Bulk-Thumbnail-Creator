using BulkThumbnailCreator;
using BulkThumbnailCreator.Enums;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.PictureClasses;
using BulkThumbnailCreator.Services;
using ImageMagick;
using Moq;

namespace Tests;

public class ProductionTests
{
    //[Fact]
    //public void Youtube_DL_Test()
    //{
    //    throw new NotImplementedException();
    //}

    //[Fact]
    //public void ProduceTextPictures_Produce_Text_Image_Text()
    //{
    //    // Arrange
    //    // First we set up our services
    //    var logService = new Mock<ILogService>();
    //    var logger = logService.Object;
    //    var jobService = new JobService();

    //    // We then set up our settings and parameters and dependencies
    //    var settings = new Settings(logger, jobService);

    //    // We dont mock this as we want to fetch some actual fonts
    //    var dirWrapper = new DirectoryWrapper();

    //    var dg = new DataGeneration(dirWrapper);
    //    var picData = new PictureData();
    //    picData.OutPutType = OutputType.Main;

    //    // first we make an actual file to read into the memory
    //    // for this we will utilize imagemagick
    //    var imageMagickSettings = new MagickReadSettings
    //    {
    //        BackgroundColor = MagickColors.None,
    //        Font = "Arial",
    //        TextGravity = Gravity.Center,
    //        Width = 800,
    //        Height = 200,
    //        FontPointsize = 72
    //    };

    //    var tempFilePath = "output_Unit_test.png";

    //    using (var image = new MagickImage("label:Hello", imageMagickSettings))
    //    {
    //        image.Wave(PixelInterpolateMethod.Spline, 5, 100);
    //        image.Write(tempFilePath);
    //    }
    //    picData.FileName = tempFilePath;

    //    // we now have an actual picture, so we set up some parameters

    //    var param1 = new ParamForTextCreation();
    //    param1.Text = "param1";


    //    param1.Boxes = DataGeneration.BuildDefaultBoxes();
    //    var param2 = new ParamForTextCreation();

    //    // Act
    //    Production.ProduceTextPictures()
    //}

    [Fact]
    public async Task CreateImage_ValidFile_ReturnsMagickImage()
    {
        // Arrange

        // first we make an actual file to read into the memory
        // for this we will utilize imagemagick
        var imageMagickSettings = new MagickReadSettings
        {
            BackgroundColor = MagickColors.None,
            Font = "Arial",
            TextGravity = Gravity.Center,
            Width = 800,
            Height = 200,
            FontPointsize = 72
        };

        var tempFilePath = "output_Unit_test.png";

        using (var image = new MagickImage("label:Hello", imageMagickSettings))
        {
            image.Wave(PixelInterpolateMethod.Spline, 5, 100);
            image.Write(tempFilePath);
        }

        var pictureData = new PictureData
        {
            FileName = tempFilePath
        };

        var mockLogService = new Mock<ILogService>();
        var jobService = new JobService();
        var settings = new Settings(mockLogService.Object, jobService);

        // Act
        var result = await Production.CreateImage(pictureData, settings);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(800, result.Width);
        //Assert.Equal(200, result.Height);
        Assert.IsType<MagickImage>(result);

        // Erase our temp file
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
        }
    }

    [Fact]
    public async Task CreateImage_InvalidFile_LogsErrorAndReturnsNull()
    {
        // Arrange

        // we will pass a file that doesnt exist to make sure it is handled gracefully
        var pictureData = new PictureData
        {
            FileName = "non_existent_file.png"
        };

        var mockLogService = new Mock<ILogService>();
        var jobService = new JobService();
        var settings = new Settings(mockLogService.Object, jobService);

        // Act
        var result = await Production.CreateImage(pictureData, settings);

        // Assert
        Assert.Null(result);
        mockLogService.Verify(log => log.LogError(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void BuildFileName_Test()
    {
        // Arrange
        var logService = new LogService();
        var jobService = new JobService();
        var settings = new Settings(logService, jobService);

        // create our test data
        var picDataMain = new PictureData { FileName = "Main", OutPutType = OutputType.Main };
        var picDataRandom = new PictureData { FileName = "Random", OutPutType = OutputType.RandomVariety };
        var picDataMeme = new PictureData { FileName = "Meme", OutPutType = OutputType.MemeVariety };
        var picDataCustom = new PictureData { FileName = "Custom", OutPutType = OutputType.Custom };

        var param1 = new ParamForTextCreation { Font = "Font1.ttf", Text = "PathTesting1" };
        var param2 = new ParamForTextCreation { Font = "Font2.ttf", Text = "PathTesting2" };

        // add parameters to our picdatas
        picDataMain.BoxParameters.Add(param1);
        picDataMain.BoxParameters.Add(param2);
        picDataRandom.BoxParameters.Add(param1);
        picDataRandom.BoxParameters.Add(param2);
        picDataMeme.BoxParameters.Add(param1);
        picDataMeme.BoxParameters.Add(param2);
        picDataCustom.BoxParameters.Add(param1);
        picDataCustom.BoxParameters.Add(param2);

        // Act
        var resultMain = Production.BuildFileName(picDataMain, settings);
        var resultRandom = Production.BuildFileName(picDataRandom, settings);
        var resultMeme = Production.BuildFileName(picDataMeme, settings);
        var resultCustom = Production.BuildFileName(picDataCustom, settings);

        // Assert
        Assert.Contains(settings.TextAddedDir, resultMain);
        Assert.Contains(picDataMain.FileName, resultMain);
        Assert.Contains(picDataMain.OutPath, resultMain);

        Assert.Contains(settings.TextAddedDir, resultRandom);
        Assert.Contains(picDataRandom.FileName, resultRandom);
        Assert.Contains(picDataRandom.OutPath, resultRandom);

        Assert.Contains(settings.TextAddedDir, resultMeme);
        Assert.Contains(picDataMeme.FileName, resultMeme);
        Assert.Contains(picDataMeme.OutPath, resultMeme);

        Assert.Contains(settings.TextAddedDir, resultCustom);
        Assert.Contains(picDataCustom.FileName, resultCustom);
        Assert.Contains(picDataCustom.OutPath, resultCustom);
    }
}
