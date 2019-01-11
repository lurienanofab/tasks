using System;
using Tasks.Models;

namespace Tasks.Jobs
{
    public static class JobLog
    {
        public static void AddEntry(string id, string path, string result, long timeTaken)
        {
            // timeTaken is in milliseconds

            var model = new JobLogModel()
            {
                Id = id,
                Path = path,
                Result = result,
                Timestamp = DateTime.Now,
                TimeTaken = timeTaken
            };

            MongoRepository.Default.AddLogEntry(model);
        }
    }
}