using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Data.Models;

namespace WebDemo.Models
{
    public class TriggeredEvent
    {
        public string Event_Type { get; set; }
        public string Country { get; set; }
        public string Alert_Type { get; set; }
        public string Computer_Name { get; set; }
        public string Recording_Name { get; set; }
        public string Event_Date { get; set; }
        public string Event_Time { get; set; }
        public string Time_Between { get; set; }
        public string Is_Active { get; set; }
        public string Duration { get; set; }
    }
}