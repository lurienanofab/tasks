using Cronos;
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
        [HttpGet, Route("")]
        public ActionResult Index()
        {
            var model = new HomeModel
            {
                Jobs = GetRecurringJobs()
            };

            return View(model);
        }

        [HttpGet, Route("job")]
        public ActionResult AddOrModifyJob(string id = null)
        {
            JobModel model = null;

            if (!string.IsNullOrEmpty(id))
                model = MongoRepository.Default.GetJob(id);
            
            if (model == null)
                model = new JobModel();

            return View(model);
        }

        [HttpPost, Route("job")]
        public ActionResult AddOrModifyJob(JobModel model)
        {
            ValidateCron(model.Cron);
            MongoRepository.Default.AddOrModifyJob(model);
            return RedirectToAction("Index");
        }

        [HttpGet, Route("job/delete")]
        public ActionResult DeleteJob(string id)
        {
            MongoRepository.Default.DeleteJob(id);
            RecurringJob.RemoveIfExists(id);
            return RedirectToAction("Index");
        }

        [HttpGet, Route("logs")]
        public ActionResult Logs()
        {
            return View();
        }

        [AllowAnonymous, Route("poke")]
        public ActionResult Poke()
        {
            return Content("ouch", "text/plain");
        }

        private IEnumerable<JobInfo> GetRecurringJobs()
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
            }).OrderBy(x => x.Id).ToList();

            return result;
        }

        private void CreateJobs()
        {
            var jobs = MongoRepository.Default.GetJobs();

            foreach(var j in jobs)
            {
                Job job = new Job(j.Path)
                {
                    Id = j.Id,
                    Method = j.Method,
                    Body = j.Body
                };

                RecurringJob.AddOrUpdate(j.Id, () => JobManager.Run(job), j.Cron, TimeZoneInfo.Local);
            }
        }

        private DateTime? ToLocalDateTime(DateTime? utc)
        {
            if (utc == null)
                return null;
            else if (utc.Value.Kind == DateTimeKind.Utc)
                return utc.Value.ToLocalTime();
            else
                return utc.Value;
        }

        /// <summary>
        /// Throws an exception if not a valid cron string.
        /// </summary>
        private void ValidateCron(string cron)
        {   
            CronExpression.Parse(cron);
        }
    }
}