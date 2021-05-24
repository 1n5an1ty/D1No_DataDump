using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebDemo.Data.Models;

namespace WebDemo.Data
{
    public class CompanyDbContext : DbContext
    {
        public virtual DbSet<AlertType> AlertTypes { get; set; }
        public virtual DbSet<CapturedPC> CapturedPCs { get; set; }
        public virtual DbSet<Recording> Recordings { get; set; }

        public CompanyDbContext() : base("CompanyDbContext")
        { 
        }
    }
}