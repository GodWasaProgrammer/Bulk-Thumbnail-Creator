namespace BulkThumbnailCreator.Interfaces;

public interface ILogService
{
    //void LogError(string message);

    //void LogWarning(string message);

    //void LogInformation(string message);

    //void LogException(string message);

    //event Action<string> LogEntryAdded;

    Task LogError(string message);

    Task LogWarning(string message);

    Task LogInformation(string message);

    Task LogException(string message);

    event Action<string> LogEntryAdded;
}
