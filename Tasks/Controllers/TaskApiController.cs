using Hangfire;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Tasks.Jobs;

namespace Tasks.Controllers
{
    public class TaskApiController : ApiController
    {
        [Route("api")]
        public string Get()
        {
            return "tasks-api";
        }

        [HttpPost, Route("api/runjob"), Route("api/job/run")]
        public async Task<object> RunJob()
        {
            dynamic post = await Request.Content.ReadAsAsync<JObject>();
            string name = post.name;

            if (name == "UpdateBilling")
            {
                int clientId = post.clientId;
                int resourceId = post.resourceId;
                int roomId = post.roomId;
                int itemId = post.itemId;
                DateTime startDate = post.startDate;
                DateTime endDate = post.endDate;
                int billingCategory = post.billingCategory;

                Job job = new Job("billing/update", "POST");
                job.AddParameter("ClientID", clientId);
                job.AddParameter("ResourceID", resourceId);
                job.AddParameter("RoomID", roomId);
                job.AddParameter("ItemID", itemId);
                job.AddParameter("StartDate", startDate);
                job.AddParameter("EndDate", endDate);
                job.AddParameter("BillingCategory", billingCategory);

                BackgroundJob.Enqueue(() => JobManager.Run(job));

                return new { name, resourceId, startDate, endDate, enqueued = true };
            }
            else
            {
                throw new NotImplementedException(string.Format("Job name {0} not implemented.", name));
            }
        }
    }
}