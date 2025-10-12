using Microsoft.EntityFrameworkCore;
using VeilingKlokKlas1Groep2.Models.Domain;

namespace VeilingKlokKlas1Groep2.Data
{
    public class VeilingKlokContext : DbContext
    {
        public VeilingKlokContext(DbContextOptions<VeilingKlokContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Koper> Kopers { get; set; }
        public DbSet<Kweker> Kwekers { get; set; }
        public DbSet<Order> Orders { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Singularization Fix (Correct)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }

            // 2. Configure Shared Primary Key (One-to-One Specialization) for Koper

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

            // 3. Configure Shared Primary Key (One-to-One Specialization) for Kweker

            // A. Define the Primary Key for Koper: It uses the AccountId property.
            modelBuilder.Entity<Kweker>()
                .HasKey(k => k.AccountId); // This is the PK definition that was missing earlier

            // B. Define the One-to-One Relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Kweker)
                .WithOne(k => k.Account)
                // C. Define the Foreign Key: It is the same column used for the PK.
                .HasForeignKey<Kweker>(k => k.AccountId)
                .IsRequired();

            // 4. Configure One-to-Many Relationship between Koper and Order

            // A. Define the PRimary Key for Order
            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id); // This is the PK definition that was missing earlier
            // B. Define the One-to-Many Relationship
            modelBuilder.Entity<Koper>()
                .HasMany(k => k.Orders)
                .WithOne(o => o.Koper)
                // C. Define the Foreign Key
                .HasForeignKey(o => o.KoperId)
                .IsRequired();

        }
    }
}