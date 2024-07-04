using BulkThumbnailCreator;
using BulkThumbnailCreator.PictureClasses;
using BulkThumbnailCreator.Services;

namespace Tests;

public class ProductionTests
{
    //[Fact]
    //public void Youtube_DL_Test()
    //{
    //throw new NotImplementedException();
    //}

    //[Fact]
    //public void ProduceTextPictures_Test()
    //{
    //throw new NotImplementedException();
    //}

    //[Fact]
    //public void CreateImage_Test()
    //{
    //throw new NotImplementedException();
    //}

    [Fact]
    public void BuildFileName_Test()
    {
        // Arrange
        var logService = new LogService();
        var jobService = new JobService();
        var settings = new Settings(logService, jobService);

        // create our test data
        var picDataMain = new PictureData { FileName = "Main" };
        var picDataRandom = new PictureData { FileName = "Random" };
        var picDataMeme = new PictureData { FileName = "Meme" };
        var picDataCustom = new PictureData { FileName = "Custom" };
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
