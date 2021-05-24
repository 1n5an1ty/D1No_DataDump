using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Data.Models;

namespace WebDemo.Models
{
    public class TriggeredEvent
    {
        public AlertType Alert { get; set; }
        public CapturedPC Computer { get; set; }
        public Recording CapturedRecording { get; set; }
        public DateTime Triggered { get; set; }
    }
}