using System;

namespace Tasks.Jobs
{
    public static class JobManager
    {
        public static void Run(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            string result = job.Run().GetAwaiter().GetResult();

            // log it
            JobLog.AddEntry(job.Path, result);
        }
    }
}
