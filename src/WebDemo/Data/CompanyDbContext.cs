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
        public virtual DbSet<Alert> Alerts { get; set; }
    }
}