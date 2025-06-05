namespace BulkThumbnailCreator.Services;

public interface IJobService
{
    // Delegater
    delegate void ResetGlobalState();
    delegate void CurrentJobHasChanged();

    // Metoder
    void RegisterResetMethod(ResetGlobalState resetGlobalState);
    void RegisterDelegateForJobChange(CurrentJobHasChanged currentJobHasChanged);
    void ResetState();
    Task<Job> RequestCurrentJob(string user);
    Task<Job> CreateJob(string videoUrl, string currentUser);
}
