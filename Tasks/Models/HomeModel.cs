using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tasks.Jobs;

namespace Tasks.Models
{
    public class HomeModel
    {
        public string SaveJobId { get; set; }
        public string SaveJobCron { get; set; }
        public string SaveJobPath { get; set; }
        public string SaveJobMethod { get; set; }
        public string SaveJobBody { get; set; }
        public IEnumerable<JobInfo> Jobs { get; set; }
    }
}