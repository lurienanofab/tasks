using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tasks.Jobs;

namespace Tasks.Models
{
    public class HomeModel
    {
        public IEnumerable<JobInfo> Jobs { get; set; }
    }
}