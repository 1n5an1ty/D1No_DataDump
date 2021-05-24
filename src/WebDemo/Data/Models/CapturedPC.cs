using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebDemo.Data.Models
{
    [Table("capture_pc")]
    public class CapturedPC
    {
        [Column("db_id")]
        public long Id { get; set; }

        [Column("username")]
        [Required]
        [StringLength(45)]
        public string Username { get; set; }

        [Column("passwor")]
        [Required]
        [StringLength(45)]
        public string Password { get; set; }

        [Column("name")]
        [Required]
        [StringLength(45)]
        public string Name { get; set; }

        [Column("teamviewer_id")]
        [Required]
        [StringLength(45)]
        public string TeamViewerId { get; set; }

        [Column("teamviewer_password")]
        [Required]
        [StringLength(45)]
        public string TeamViewerPassword { get; set; }

        [Column("country_code")]
        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [Column("last_activity_change")]
        public DateTime? LastActivityChange { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("last_settings_read")]
        public DateTime? LastSettingsRead { get; set; }

        [Column("add_datetime")]
        public DateTime Added { get; set; }

        [Column("external_ip")]
        [Required]
        [StringLength(45)]
        public string ExternalIpAddress { get; set; }

        [Column("internal_ip")]
        [Required]
        [StringLength(45)]
        public string InternalIpAddress { get; set; }

        [Column("windows_username")]
        [Required]
        [StringLength(45)]
        public string WindowsUsername { get; set; }

        [Column("windows_password")]
        [Required]
        [StringLength(45)]
        public string WindowsPassword { get; set; }

        [Column("indefinitely_offline")]
        public bool IndefinitlyOffline { get; set; }
    }
}