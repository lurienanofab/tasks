using System;
using System.Diagnostics;

namespace Tasks.Jobs
{
    public static class JobManager
    {
        public static void Run(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            Stopwatch sw = Stopwatch.StartNew();
            string result = job.Run();
            sw.Stop();

            // log it
            JobLog.AddEntry(job.Id, job.Path, result, Convert.ToInt32(sw.ElapsedMilliseconds));
        }
    }
}
