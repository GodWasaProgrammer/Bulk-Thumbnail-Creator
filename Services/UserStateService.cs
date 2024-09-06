namespace BulkThumbnailCreator.Services;

public class UserStateService
{
    /// <summary>
    /// we will store all our jobs here
    /// So we can fetch them on page refreshes
    public static List<Job> UserJobs { get; set; } = [];

    /// <summary>
    /// Saves or updates a job in the list
    /// <param name="job"></param>
    public static void AddJob(Job job)
    {
        UserJobs.Add(job);
    }

    /// <summary>
    /// To delete jobs from the list
    /// <param name="job"></param>
    public static void RemoveJob(Job job)
    {
        UserJobs.Remove(job);
    }

    /// <summary>
    /// Fetches a job from the list, to be able to Maintain state between page refreshes
    /// <returns>the job</returns>
    public static Job GetJob(string user)
    {
        return UserJobs.LastOrDefault(x => x.User == user);
    }
}
