using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class AlertRange
    {
        public int Alert_Type_Id { get; set; }
        public int Capture_PC_Id { get; set; }
        public int Recording_Id { get; set; }
        public object Time_Spent_In_Alert_State { get; set; }
        public bool Currently_In_Alert_State { get; set; }
        public IEnumerable<AlertTimeRange> Time_Ranges { get; set; }
    }
}