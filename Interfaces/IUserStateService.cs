namespace BulkThumbnailCreator.Services;

public interface IUserStateService
{
    List<Job> UserJobs { get; set; }
    void AddJob(Job job);
    void RemoveJob(Job job);
    Job GetJob(string user);
}
