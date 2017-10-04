using OnlineServices.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;

namespace Tasks.Jobs
{
    public class Job
    {
        // a job calls a webapi using GET or POST, that is all
        private readonly Dictionary<string, string> _data;

        public string Path { get; set; }
        public string Method { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Data { get { return _data; } }

        public Job()
        {
            Path = string.Empty;
            Method = "GET";
            _data = new Dictionary<string, string>();
        }

        public Job(string path, string method = "GET")
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            if (!new[] { "GET", "POST" }.Contains(method))
                throw new ArgumentOutOfRangeException("method");

            Path = path;
            Method = method;
            _data = new Dictionary<string, string>();
        }

        public Job(string path, IDictionary<string, string> data)
        {
            Path = path;
            _data = new Dictionary<string, string>(data);
        }

        public void AddParameter(string key, object value)
        {
            _data.Add(key, value.ToString());
        }

        public async Task<string> Run()
        {
            try
            {
                using (var client = new ApiClient(ConfigurationManager.AppSettings["ApiHost"]))
                {
                    string result = null;

                    if (Method == "GET")
                        result = await client.Get(Path);
                    else if (Method == "POST")
                        result = await client.Post(Path, _data);
                    else
                        throw new NotImplementedException();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}