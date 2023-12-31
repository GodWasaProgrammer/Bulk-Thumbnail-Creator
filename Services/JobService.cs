using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bulk_Thumbnail_Creator.Services
{
    public class JobService
    {
        /// <summary>
        /// not implemented yet
        /// </summary>
        private string _User;
        public string User { get { return _User; } set { _User = value; } }

        // store the current job that is active
        public Job CurrentJob { get; set; }

        // store all the jobs that have been created
        public List<Job> Jobs { get; set; } = new List<Job>();

        public Task<Job> RequestCurrentJob()
        {
            return Task.FromResult(CurrentJob);
        }

        public Task<Job> CreateJob(string videoUrl)
        {
            Job job = new(videoUrl);

            // set the current job to the job that was just created
            // so we are able to lift it on demand
            CurrentJob = job;

            // add the job the joblist
            Jobs.Add(job);

            // will return the job if it was created successfully, otherwise will return a null object
            return job != null ? Task.FromResult(job) : null;
        }


    }

}
