using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class RequestFilters
    {
        public EventType? EventType { get; set; }
        public int? MinutesLookback { get; set; }
        public int? AlertTypeId { get; set; }
        public long? CapturedPCId { get; set; }
        public long? RecordingId { get; set; }
        public string CountryCode { get; set; }
    }
}