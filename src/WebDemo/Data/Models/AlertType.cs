using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDemo.Data.Models
{
    [Table("alert_type")]
    public class AlertType
    {
        [Column("db_id")]
        public int Id { get; set; }

        [Column("alert_name")]
        [Required]
        [StringLength(64)]
        public string Name { get; set; }
    }
}