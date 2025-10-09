using Microsoft.EntityFrameworkCore;
using WebProject_Klas1_Groep2.Models;

namespace WebProject_Klas1_Groep2.Data
{
    public class VeilingKlokContext : DbContext
    {
        public VeilingKlokContext(DbContextOptions<VeilingKlokContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Koper> Kopers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Singularization Fix (Correct)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }

            // 2. Configure Shared Primary Key (One-to-One Specialization)

            // A. Define the Primary Key for Koper: It uses the AccountId property.
            modelBuilder.Entity<Koper>()
                .HasKey(k => k.AccountId); // This is the PK definition that was missing earlier

            // B. Define the One-to-One Relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Koper)
                .WithOne(k => k.Account)
                // C. Define the Foreign Key: It is the same column used for the PK.
                .HasForeignKey<Koper>(k => k.AccountId)
                .IsRequired();
        }
    }
}