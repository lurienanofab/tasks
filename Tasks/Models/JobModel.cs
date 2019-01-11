namespace Tasks.Models
{
    public class JobModel
    {
        public string Id { get; set; }
        public string Cron { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
    }
}