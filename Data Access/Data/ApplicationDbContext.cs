using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Data
{
   public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions):base(dbContextOptions)
        {
            
        }
         public virtual DbSet<Owner> owners { get; set; }
        public virtual DbSet<House> houses { get; set; }
        public virtual DbSet<Heater> heaters { get; set; }
        public virtual DbSet<MonthlyCostReport> monthlyCostReports { get; set; }
        public virtual  DbSet<SensorReading> sensorReadings { get; set; }
        public virtual  DbSet<User> Users { get; set; }

    }
}
