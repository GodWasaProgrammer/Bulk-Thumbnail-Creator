namespace BulkThumbnailCreator.Interfaces
{
    public interface IDirectoryWrapper
    {
        string[] GetFiles(string path, string searchPattern);
    }
}
