using System;

namespace Tasks.Models
{
    public class JobLogModel
    {
        public string Path { get; set; }
        public string Result { get; set; }
        public DateTime Timestamp { get; set; }
    }
}