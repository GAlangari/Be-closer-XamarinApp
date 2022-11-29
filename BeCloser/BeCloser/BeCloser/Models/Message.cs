using System;
using System.Collections.Generic;
using System.Text;

namespace BeCloser.Models
{
    public class Message
    {
        public string to { set; get; }
        public string from { set; get; }
        public string content { set; get; }
        public DateTime time { set; get; }
        public TimeSpan Timespan { get; set; }
        public string Minutes => Timespan.Minutes.ToString("00");
        public string Seconds => Timespan.Seconds.ToString("00");
        public bool isTeacher { get; set; }
    }
}
