using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptureFingerTextVirdi.Mapping;
using CaptureFingerTextVirdi.Models;

namespace CaptureFingerTextVirdi.Context
{
    public class CaptureFingerTextContext : DbContext
    {
        public CaptureFingerTextContext() : base(@"connection string here")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserImported> UserImported { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMapping());
            modelBuilder.Configurations.Add(new UserImportedMapping());
        }
    }
}
