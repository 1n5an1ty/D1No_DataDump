using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.JSON;

namespace WebDemo.Models
{
    public class AlertTimeRange
    {
        [JsonConverter(typeof(FileTime))]
        public DateTime Start { get; set; }

        [JsonConverter(typeof(FileTime))]
        public DateTime End { get; set; }
    }
}