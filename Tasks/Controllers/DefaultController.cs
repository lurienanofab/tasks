using Hangfire;
using LNF.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tasks.Jobs;
using Tasks.Models;

namespace Tasks.Controllers
{
    [MultiAuthorize]
    public class DefaultController : ApiController
    {
        [Route("api")]
        public string Get()
        {
            return "tasks-api";
        }

        [HttpPost, Route("api/runjob"), Route("api/job/run")]
        public async Task<object> RunJob()
        {
            dynamic body = await Request.Content.ReadAsAsync<JObject>();
            string name = body.name;

            if (name == "UpdateBilling")
            {
                int clientId = body.clientId;
                int resourceId = body.resourceId;
                int roomId = body.roomId;
                int itemId = body.itemId;
                DateTime startDate = body.startDate;
                DateTime endDate = body.endDate;
                int billingCategory = body.billingCategory;

                Job job = new Job("webapi/billing/update", JsonConvert.SerializeObject(new
                {
                    ClientID = clientId,
                    ResourceID = resourceId,
                    RoomID = roomId,
                    ItemID = itemId,
                    StartDate = startDate,
                    EndDate = endDate,
                    BillingCategory = billingCategory
                }));

                BackgroundJob.Enqueue(() => JobManager.Run(job));

                return new { name, resourceId, startDate, endDate, enqueued = true };
            }
            else
            {
                throw new NotImplementedException(string.Format("Job name {0} not implemented.", name));
            }
        }

        [Route("api/log")]
        public IEnumerable<JobLogModel> GetLogs(DateTime after, string id = null)
        {
            var result = MongoRepository.Default.GetLogEntries(after, id).OrderByDescending(x => x.Timestamp);
            return result;
        }

        [HttpGet, Route("api/log/clear")]
        public long ClearLogs(DateTime? after = null, string id = null)
        {
            var result = MongoRepository.Default.DeleteLogs(after, id);
            return result;
        }

        [Route("api/cron/{method}")]
        public string GetCronString(string method, int month = 1, int day = 1, int hour = 0, int minute = 0, int interval = 1, DayOfWeek dow = DayOfWeek.Sunday)
        {
            if (month < 1 || month > 12)
                throw new Exception($"Invalid month: {month}");

            if (day < 1 || day > 31)
                throw new Exception($"Invalid day: {day}");

            if (hour < 0 || hour > 23)
                throw new Exception($"Invalid hour: {hour}");

            if (minute < 0 || minute > 59)
                throw new Exception($"Invalid minute: {minute}");

            string result;

            switch (method)
            {
                case "daily":
                    result = Cron.Daily(hour, minute);
                    break;
                case "day-interval":
                    result = Cron.DayInterval(interval);
                    break;
                case "hour-interval":
                    result = Cron.HourInterval(interval);
                    break;
                case "hourly":
                    result = Cron.Hourly(minute);
                    break;
                case "minute-interval":
                    result = Cron.MinuteInterval(interval);
                    break;
                case "minutely":
                    result = Cron.Minutely();
                    break;
                case "month-interval":
                    result = Cron.MonthInterval(interval);
                    break;
                case "monthly":
                    result = Cron.Monthly(day, hour, minute);
                    break;
                case "weekly":
                    result = Cron.Weekly(dow, hour, minute);
                    break;
                case "yearly":
                    result = Cron.Yearly(month, day, hour, minute);
                    break;
                default:
                    throw new Exception($"Unknown method: {method}");
            }

            return result;
        }

        [HttpGet, Route("api/cron/eval")]
        public CronEvalResult EvalCron(string cron)
        {
            IRestClient client = new RestClient("https://cronexpressiondescriptor.azurewebsites.net");
            IRestRequest req = new RestRequest("api/descriptor", Method.GET);
            req.AddQueryParameter("expression", cron);
            req.AddQueryParameter("locale", "en-US");
            IRestResponse<CronEvalResult> resp = client.Execute<CronEvalResult>(req);
            return resp.Data;
        }
    }
}