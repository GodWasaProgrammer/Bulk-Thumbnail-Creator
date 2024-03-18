namespace BulkThumbnailCreator.Interfaces;

public interface ILogService
{
    Task LogError(string message);

    Task LogWarning(string message);

    Task LogInformation(string message);

    Task LogException(string message);

    event Action<string> LogEntryAdded;
}
