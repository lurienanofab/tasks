using System;

namespace Tasks.Models
{
    public class JobLogModel
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string Result { get; set; }
        public DateTime Timestamp { get; set; }
        public long TimeTaken { get; set; }
    }
}