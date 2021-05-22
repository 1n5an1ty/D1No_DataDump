using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDemo.Data.Models
{
    public class Alert
    {
        [Column("db_id")]
        public int Id { get; set; }

        [Column("alert_name")]
        public string Name { get; set; }
    }
}