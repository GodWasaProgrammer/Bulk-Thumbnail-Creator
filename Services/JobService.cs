using System.Collections.Generic;
using System.IO;

namespace Bulk_Thumbnail_Creator.Services
{
    public class JobService
    {
        public JobService(string user)
        {
            user = User;
            CreateOutPutFileUrlList();
        }

        /// <summary>
        /// not implemented yet
        /// </summary>
        private string _User;
        public string User { get { return _User; } set { _User = value; } }

        // store the current job that is active
        public Job CurrentJob { get; set; }

        // store all the jobs that have been created
        public List<Job> JobList { get; set; } = new List<Job>();

        public void CreateJob(string url)
        {
            CurrentJob = new Job(url);
            JobList.Add(CurrentJob);
        }

        string[] outPutDirUrls;

        public void CreateOutPutFileUrlList()
        {
            outPutDirUrls = Directory.GetFiles(Settings.OutputDir);
        }

    }

}
