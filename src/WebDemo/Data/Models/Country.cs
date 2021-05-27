using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDemo.Data.Models
{
    [Table("country")]
    public class Country
    {
        [Column("country_code")]
        [Key]
        public string CountryCode { get; set; }

        [Column("country_name")]
        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }
    }
}