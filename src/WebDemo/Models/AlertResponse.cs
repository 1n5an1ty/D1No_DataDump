using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class AlertResponse
    {
        public IEnumerable<AlertEvent> Events { get; set; }
        public IEnumerable<AlertRange> Ranges { get; set; }
    }
}