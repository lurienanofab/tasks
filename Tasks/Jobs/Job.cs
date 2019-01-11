using LNF.CommonTools;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;

namespace Tasks.Jobs
{
    public class Job
    {
        // A job calls a webapi using GET or POST, that is all.

        public string Id { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }

        // There must be a parameterless contructor so Hangire can create a new object.
        public Job()
        {
            Path = string.Empty;
            Method = "GET";
        }

        public Job(string path)
        {
            Path = path;
            Method = "GET";
        }

        public Job(string path, string body)
        {
            Path = path;
            Method = "POST";
            Body = body;
        }

        public string Run()
        {
            if (string.IsNullOrEmpty(Path))
                throw new Exception("Path cannot be empty.");

            var method = GetMethod();
            var host = Utility.GetRequiredAppSetting("ApiBaseUrl");
            var username = Utility.GetRequiredAppSetting("BasicAuthUsername");
            var password = Utility.GetRequiredAppSetting("BasicAuthPassword");

            IRestClient client = new RestClient(host)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };

            IRestRequest req = new RestRequest(Path, method);

            if (method == RestSharp.Method.POST && !string.IsNullOrEmpty(Body))
            {
                req.RequestFormat = DataFormat.Json;
                req.AddParameter("application/json", Body, ParameterType.RequestBody);
            }

            IRestResponse resp = client.Execute(req);

            if (resp.IsSuccessful)
            {
                string result = JsonConvert.DeserializeObject<string>(resp.Content);
                return result;
            }
            else
            {
                if (resp.ErrorException == null)
                {
                    var err = JsonConvert.DeserializeAnonymousType(resp.Content, new { Message = "" });

                    if (!string.IsNullOrEmpty(err.Message))
                        throw new Exception(err.Message);
                    else
                        throw new Exception(resp.Content);
                }
                else
                {
                    throw resp.ErrorException;
                }
            }
        }

        private Method GetMethod()
        {
            switch (Method)
            {
                case "GET":
                    return RestSharp.Method.GET;
                case "POST":
                    return RestSharp.Method.POST;
                default:
                    throw new Exception($"Method not supported: {Method}.");
            }
        }
    }
}