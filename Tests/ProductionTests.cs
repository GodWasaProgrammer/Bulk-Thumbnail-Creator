using BulkThumbnailCreator;
using BulkThumbnailCreator.Interfaces;
using BulkThumbnailCreator.PictureClasses;
using BulkThumbnailCreator.Services;

namespace Tests;

public class ProductionTests
{
    [Fact]
    public void Youtube_DL_Test()
    {

    }

    [Fact]
    public void ProduceTextPictures_Test()
    {

    }

    [Fact]
    public void CreateImage_Test()
    {

    }

    [Fact]
    public void BuildFileName_Test()
    {
        var logService = new LogService();
        var jobService = new JobService();
        var settings = new Settings(logService,jobService);

        var picdata = new PictureData { FileName = "PathTesting" };
        var param1 = new ParamForTextCreation { Font = "Font1.ttf", Text = "PathTesting" };

    }
}
