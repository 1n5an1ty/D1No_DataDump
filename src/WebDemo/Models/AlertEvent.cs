using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class AlertEvent
    {
        public int Alert_Type_Id { get; set; }
        public int Capture_PC_Id { get; set; }
        public int Recording_Id { get; set; }
        public object Average_Time_Between_Events { get; set; }
        public List<object> Timestamps { get; set; }
    }
}