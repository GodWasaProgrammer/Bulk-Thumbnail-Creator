namespace BulkThumbnailCreator.Interfaces
{
    public interface ILogService
    {
        void LogError(string message);

        void LogWarning(string message);

        void LogInformation(string message);

        void LogException(string message);

        event Action<string> LogEntryAdded;
    }

}
