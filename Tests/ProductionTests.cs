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

        var picdata = new PictureData { FileName = "PathTesting" };
        var param1 = new ParamForTextCreation { Font = "Font1.ttf", Text = "PathTesting1" };
        var param2 = new ParamForTextCreation { Font = "Font2.ttf", Text = "PathTesting2" };
        picdata.BoxParameters.Add(param1);
        picdata.BoxParameters.Add(param2);

        // Act
        var result = Production.BuildFileName(picdata, settings);

        // Assert
        Assert.Contains(settings.TextAddedDir, result);
        Assert.Contains("PathTesting", result);
        Assert.Contains(picdata.OutPath, result);
    }
}
