using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrgCheck.Models
{
    public class DataProtectionKeyContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options)
            : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataProtectionKey>(entity =>
            {
                entity.ToTable("dataprotectionkeys", "orgcheck");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FriendlyName).HasColumnName("friendlyname");
                entity.Property(e => e.Xml).HasColumnName("xml");
            });
        }
    }
}
