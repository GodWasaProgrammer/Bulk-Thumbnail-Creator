namespace BulkThumbnailCreator.Diagnostics;

public class UserStateServiceInstance : IUserStateService
{
    public List<Job> UserJobs { get; set; } = new();

    public void AddJob(Job job)
    {
        UserJobs.Add(job);
    }

    public void RemoveJob(Job job)
    {
        UserJobs.Remove(job);
    }

    public Job GetJob(string user)
    {
        return UserJobs.LastOrDefault(x => x.User == user);
    }
}
