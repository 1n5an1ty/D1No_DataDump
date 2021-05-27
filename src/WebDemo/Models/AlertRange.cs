using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.JSON;

namespace WebDemo.Models
{
    public class AlertRange
    {
        public int Alert_Type_Id { get; set; }
        public long Capture_PC_Id { get; set; }
        public long Recording_Id { get; set; }
        [JsonConverter(typeof(TimeFrame))]
        public TimeSpan Time_Spent_In_Alert_State { get; set; }
        public bool Currently_In_Alert_State { get; set; }
        public IEnumerable<AlertTimeRange> Time_Ranges { get; set; }
    }
}