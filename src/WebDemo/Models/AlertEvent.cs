using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.JSON;

namespace WebDemo.Models
{
    public class AlertEvent
    {
        public int Alert_Type_Id { get; set; }
        public long Capture_PC_Id { get; set; }
        public long Recording_Id { get; set; }
        [JsonConverter(typeof(TimeFrame))]
        public TimeSpan Average_Time_Between_Events { get; set; }
        public List<long> Timestamps { get; set; }
    }
}