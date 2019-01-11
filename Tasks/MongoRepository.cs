using Hangfire;
using MongoDB.Bson;
using MongoDB.Driver;
using Cronos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tasks.Models;

namespace Tasks
{
    public class MongoRepository
    {
        public static MongoRepository Default { get; }

        static MongoRepository()
        {
            Default = new MongoRepository();
        }

        private readonly IMongoClient _client;

        private MongoRepository()
        {
            _client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
        }

        private IMongoCollection<T> GetCollection<T>(string name)
        {
            var db = _client.GetDatabase("tasks");
            var col = db.GetCollection<T>(name);
            return col;
        }

        public void AddLogEntry(JobLogModel model)
        {
            var col = GetCollection<BsonDocument>("logs");

            var doc = new BsonDocument()
            {
                { "Id", new BsonString(model.Id) },
                { "Path", new BsonString(model.Path)},
                { "Result", new BsonString(model.Result) },
                { "Timestamp", new BsonDateTime(model.Timestamp) },
                { "TimeTaken", new BsonInt64(model.TimeTaken) }
            };

            col.InsertOne(doc);
        }

        public IEnumerable<JobLogModel> GetLogEntries(DateTime after, string id = null)
        {
            var col = GetCollection<BsonDocument>("logs");

            FilterDefinition<BsonDocument> def;

            if (string.IsNullOrEmpty(id))
                def = WhereJobLogTimestampAfter(after);
            else
                def = Builders<BsonDocument>.Filter.And(WhereJobLogTimestampAfter(after), WhereJobIdEquals(id));

            var cursor = col.Find(def);
            var docs = cursor.ToList();

            var result = docs.Select(CreateJobLogModel).ToList();

            return result;
        }

        public long DeleteLogs(DateTime? after = null, string id = null)
        {
            var col = GetCollection<BsonDocument>("logs");

            FilterDefinition<BsonDocument> def;

            if (after.HasValue)
                def = WhereJobLogTimestampAfter(after.Value);
            else
                def = Builders<BsonDocument>.Filter.Empty;

            if (!string.IsNullOrEmpty(id))
                def = Builders<BsonDocument>.Filter.And(def, WhereJobIdEquals(id));

            return col.DeleteMany(def).DeletedCount;
        }

        public void AddOrModifyJob(JobModel model)
        {
            var col = GetCollection<BsonDocument>("jobs");

            var doc = new BsonDocument()
            {
                { "Id", new BsonString(model.Id) },
                { "Cron", new BsonString(model.Cron) },
                { "Method", new BsonString(model.Method) },
                { "Path", new BsonString(model.Path) },
                { "Body", new BsonString(UnformatJson(model.Body)) }
            };

            var opts = new FindOneAndReplaceOptions<BsonDocument, BsonDocument>()
            {
                IsUpsert = true
            };

            col.FindOneAndReplace(WhereJobIdEquals(model.Id), doc, opts);
        }

        public JobModel GetJob(string id)
        {
            var col = GetCollection<BsonDocument>("jobs");
            var doc = col.Find(WhereJobIdEquals(id)).FirstOrDefault();
            var result = CreateJobModel(doc);
            return result;
        }

        public IEnumerable<JobModel> GetJobs()
        {
            var col = GetCollection<BsonDocument>("jobs");

            var docs = col.Find(Builders<BsonDocument>.Filter.Empty).ToList();

            var result = docs.Select(CreateJobModel).ToList();

            return result;
        }

        public long DeleteJob(string id)
        {
            var col = GetCollection<BsonDocument>("jobs");
            return col.DeleteOne(WhereJobIdEquals(id)).DeletedCount;
        }

        private FilterDefinition<BsonDocument> WhereJobIdEquals(string id)
        {
            return Builders<BsonDocument>.Filter.Eq(x => x["Id"], id);
        }

        private FilterDefinition<BsonDocument> WhereJobLogTimestampAfter(DateTime after)
        {
            return Builders<BsonDocument>.Filter.Gte(x => x["Timestamp"], after);
        }

        private JobModel CreateJobModel(BsonDocument doc)
        {
            if (doc == null) return null;

            return new JobModel()
            {
                Id = doc["Id"].AsString,
                Cron = doc["Cron"].AsString,
                Method = doc["Method"].AsString,
                Path = doc["Path"].AsString,
                Body = doc["Body"].AsString
            };
        }

        private JobLogModel CreateJobLogModel(BsonDocument doc)
        {
            if (doc == null) return null;

            return new JobLogModel()
            {
                Id = doc["Id"].AsString,
                Path = doc["Path"].AsString,
                Result = doc["Result"].AsString,
                Timestamp = doc["Timestamp"].ToUniversalTime().ToLocalTime(),
                TimeTaken = doc["TimeTaken"].AsInt64
            };
        }

        /// <summary>
        /// Unformats a json string, removing line breaks, tabs, and extra white space.
        /// </summary>
        private string UnformatJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            var jobj = JObject.Parse(json);

            return JsonConvert.SerializeObject(jobj);
        }
    }
}