using Hangfire;
using Hangfire.Mongo;
using MongoDB.Driver;
using System.Configuration;
using System.Web.Hosting;

namespace Tasks
{
    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper() { }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                var connstr = ConfigurationManager.AppSettings["MongoConnectionString"];
                var migrationOptions = new MongoMigrationOptions { Strategy = MongoMigrationStrategy.Drop, BackupStrategy = MongoBackupStrategy.Collections };
                var storageOptions = new MongoStorageOptions { MigrationOptions = migrationOptions };
                var mongoSettings = MongoClientSettings.FromConnectionString(connstr);
                JobStorage.Current = new MongoStorage(mongoSettings, "hangfire", storageOptions);
                // Specify other options here

                _backgroundJobServer = new BackgroundJobServer();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (_backgroundJobServer != null)
                {
                    _backgroundJobServer.Dispose();
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            Stop();
        }
    }
}