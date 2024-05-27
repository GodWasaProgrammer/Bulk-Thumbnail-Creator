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
            var jobToUpdate = UserJobs.Where(x => x.JobID == job.JobID).FirstOrDefault();

            if (jobToUpdate != null)
            {
                jobToUpdate.VideoName = job.VideoName;
                jobToUpdate.VideoPath = job.VideoPath;
                jobToUpdate.VideoUrl = job.VideoUrl;
                jobToUpdate.FrontLineUpUrls = job.FrontLineUpUrls;
                jobToUpdate.VarietyUrls = job.VarietyUrls;
                jobToUpdate.State = job.State;
                jobToUpdate.LogEntries = job.LogEntries;
                jobToUpdate.TextToPrint = job.TextToPrint;
                jobToUpdate.User = job.User;
                jobToUpdate.Settings = job.Settings;
                jobToUpdate.JobID = job.JobID;
                jobToUpdate.PictureData = job.PictureData;
            }
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
        /// <returns>the job</returns>
        public static Job GetJob(string user)
        {
            Job debug = UserJobs.LastOrDefault(x => x.User == user);

            return UserJobs.LastOrDefault(x => x.User == user);
        }
    }
}
