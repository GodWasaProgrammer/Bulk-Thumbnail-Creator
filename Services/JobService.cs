namespace BulkThumbnailCreator.Services
{
    public class JobService
    {
        // representing methods with no parameters and no return value
        public delegate void ResetGlobalState();

        // the private field that stores the delegate
        private ResetGlobalState _resetDelegate;

        // here we will register methods to be called when the reset method is called
        public void RegisterResetMethod(ResetGlobalState resetGlobalState)
        {
            _resetDelegate += resetGlobalState;
        }

        public void RegisterDelegateForJobChange(CurrentJobHasChanged currentJobHasChanged)
        {
            _currentJobHasChanged += currentJobHasChanged;
        }

        public delegate void CurrentJobHasChanged();

        private CurrentJobHasChanged _currentJobHasChanged;

        // here is our actual global reset
        public void ResetState()
        {
            _resetDelegate?.Invoke();
        }

        // store the current job that is active
        public Job CurrentJob { get; set; }


        public Task<Job> RequestCurrentJob(string user)
        {
            CurrentJob = UserStateService.GetJob(user);

            return Task.FromResult(CurrentJob);
        }

        public Task<Job> CreateJob(string videoUrl, string currentUser)
        {
            Job job = new(videoUrl, currentUser);

            // set the current job to the job that was just created
            // so we are able to lift it on demand
            CurrentJob = job;
            CurrentJob.User = currentUser;

            // add the job the joblist
            UserStateService.AddJob(CurrentJob);
            _currentJobHasChanged.Invoke();

            // will return the job if it was created successfully, otherwise will return a null object
            return job != null ? Task.FromResult(job) : null;
        }

        public Job Reset()
        {
            // reset the current job
            CurrentJob = null;

            return CurrentJob;
        }
    }
}
