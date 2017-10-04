using LNF.Cache;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace Tasks.Jobs
{
    public static class JobLog
    {
        public static void AddEntry(string path, string result)
        {
            var mongo = MongoRepository.Default.GetClient();
            var db = mongo.GetDatabase("logs");
            var col = db.GetCollection<BsonDocument>("job");

            BsonDocument document = new BsonDocument();

            document
                .Add("Path", new BsonString(path))
                .Add("Result", new BsonString(result))
                .Add("Timestamp", new BsonDateTime(DateTime.Now));

            col.InsertOne(document);
        }
    }
}