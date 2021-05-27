using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Data.Models;

namespace WebDemo.Models
{
    public class TriggeredEvent
    {
        public EventType Type { get; set; }
        public string Country { get; set; }
        public AlertType Alert { get; set; }
        public CapturedPC Computer { get; set; }
        public Recording CapturedRecording { get; set; }
        public DateTime Triggered { get; set; }
        public TimeSpan AvgTimeBetweenEvents { get; set; }
        public bool? IsActive { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}