using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tasks.Jobs;
using Tasks.Models;

namespace Tasks.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index(HomeModel model)
        {
            model.Jobs = GetRecurringJobs().OrderBy(x => x.Id);
            return View(model);
        }

        [AllowAnonymous, Route("poke")]
        public ActionResult Poke()
        {
            return Content("ouch", "text/plain");
        }

        public IEnumerable<JobInfo> GetRecurringJobs()
        {
            CreateJobs();

            List<RecurringJobDto> list = JobStorage.Current.GetConnection().GetRecurringJobs();

            IEnumerable<JobInfo> result = list.Select(x => new JobInfo()
            {
                Id = x.Id,
                Cron = x.Cron,
                CreatedAt = ToLocalDateTime(x.CreatedAt),
                LastExecution = ToLocalDateTime(x.LastExecution),
                NextExecution = ToLocalDateTime(x.NextExecution)
            }).ToList();

            return result;
        }

        private void CreateJobs()
        {
            Job job;

            job = new Job("scheduler/service/task-5min");
            RecurringJob.AddOrUpdate("FiveMinute", () => JobManager.Run(job), Cron.MinuteInterval(5), TimeZoneInfo.Local);

            job = new Job("scheduler/service/task-daily");
            RecurringJob.AddOrUpdate("Daily", () => JobManager.Run(job), Cron.Daily(), TimeZoneInfo.Local);

            job = new Job("scheduler/service/task-monthly");
            RecurringJob.AddOrUpdate("Monthly", () => JobManager.Run(job), Cron.Monthly(), TimeZoneInfo.Local);
        }

        public DateTime? ToLocalDateTime(DateTime? utc)
        {
            if (utc == null)
                return null;
            else if (utc.Value.Kind == DateTimeKind.Utc)
                return utc.Value.ToLocalTime();
            else
                return utc.Value;
        }
    }
}