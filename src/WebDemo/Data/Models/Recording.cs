using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDemo.Data.Models
{
    [Table("recording")]
    public class Recording
    {
        [Column("db_id")]
        public long Id { get; set; }

        [Column("recording_name")]
        [Required]
        [StringLength(45)]
        public string Name { get; set; }
    }
}