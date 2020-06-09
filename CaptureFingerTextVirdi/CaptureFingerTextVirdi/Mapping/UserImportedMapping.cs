using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptureFingerTextVirdi.Models;

namespace CaptureFingerTextVirdi.Mapping
{
    public class UserImportedMapping : EntityTypeConfiguration<UserImported>
    {
        public UserImportedMapping()
        {
            ToTable("UserImported");

            this.HasKey(u => u.Id);

            this.Property(u => u.UserId)
                .HasMaxLength(100)
                .IsRequired();

            this.Property(u => u.UserName)
                .HasMaxLength(100)
                .IsRequired();

            this.Property(u => u.Rfid)
                .HasMaxLength(100)
                .IsOptional();

            this.Property(u => u.FingerText)
                .HasColumnType("VARBINARY(MAX)")
                .IsOptional();

            Property(u => u.Name)
                .IsOptional();

            Property(u => u.Email)
                .IsOptional();

            Property(u => u.Document)
                .IsOptional();

            Property(u => u.PhoneNumber)
                .IsOptional();
        }
    }
}
