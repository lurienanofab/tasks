using System;

namespace Tasks.Jobs
{
    public class JobInfo
    {
        public string Id { get; set; }
        public string Cron { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastExecution { get; set; }
        public DateTime? NextExecution { get; set; }
    }
}
