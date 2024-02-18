namespace BulkThumbnailCreator.Services
{
    public class UserStateService
    {
        /// <summary>
        /// we will store all our jobs here
        /// So we can fetch them on page refreshes
        /// </summary>
        public static List<Job> UserJobs { get; set; } = [];

        /// <summary>
        /// Saves or updates a job in the list
        /// </summary>
        /// <param name="job"></param>
        public static void AddJob(Job job)
        {
            UserJobs.Add(job);
        }

        public static void UpdateJob(Job job)
        {
            Job JobToUpdate = UserJobs.Where(x => x.JobID == job.JobID).FirstOrDefault();

            JobToUpdate.VideoName = job.VideoName;
            JobToUpdate.VideoPath = job.VideoPath;
            JobToUpdate.VideoUrl = job.VideoUrl;
            JobToUpdate.FrontLineUpUrls = job.FrontLineUpUrls;
            JobToUpdate.VarietyUrls = job.VarietyUrls;
            JobToUpdate.State = job.State;
            JobToUpdate.LogEntries = job.LogEntries;
            JobToUpdate.TextToPrint = job.TextToPrint;
            JobToUpdate.User = job.User;
            JobToUpdate.TextToPrint = job.TextToPrint;
            JobToUpdate.Settings = job.Settings;
            JobToUpdate.JobID = job.JobID;
            JobToUpdate.PictureDatas = job.PictureDatas;
        }

        /// <summary>
        /// To delete jobs from the list
        /// </summary>
        /// <param name="job"></param>
        public static void RemoveJob(Job job)
        {
            UserJobs.Remove(job);
        }

        /// <summary>
        /// Fetches a job from the list, to be able to Maintain state between page refreshes
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns></returns>
        public static Job GetJob(string User)
        {
            Job DebugJob = UserJobs.Where(x => x.User == User).FirstOrDefault();

            return UserJobs.Where(x => x.User == User).FirstOrDefault();
        }
    }
}
