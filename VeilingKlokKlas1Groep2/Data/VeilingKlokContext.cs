using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Models.Domain;

namespace VeilingKlokApp.Data
{
    public class VeilingKlokContext(DbContextOptions<VeilingKlokContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Koper> Kopers { get; set; }
        public DbSet<Kweker> Kwekers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Veilingmeester> Veilingmeesters { get; set; }
        public DbSet<VeilingKlok> Veilingklokken { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Top level call to base method
            base.OnModelCreating(modelBuilder);

            // 1. Specialization Strategy: Table-Per-Type (TPT)
            // This ensures separate tables for Account, Kopers, and Kwekers,
            // linked by the primary key, enforcing the specialization naturally.
            modelBuilder.Entity<Account>()
                .UseTptMappingStrategy();

            // 2. Unique Email Constraint
            // Make it easier to find accounts by email and enforce uniqueness.
            // Creates a unique index on the 'email' column in the base 'Account' table.
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // 3. Define the One-to-Many Relationship Koper to Orders
            modelBuilder.Entity<Koper>()
                .HasMany(k => k.Orders)
                .WithOne(o => o.Koper)
                .HasForeignKey(o => o.KoperId)
                .IsRequired();

            // 4. One-to-many Relationship for Buyer orders 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Koper)             // Order has one Koper
                .WithMany(k => k.Orders)          // Koper has many Orders
                .HasForeignKey(o => o.KoperId)    // Uses the KoperId field as the Foreign Key
                .OnDelete(DeleteBehavior.Restrict); // Important: Prevents cascading delete issues

            // 5. One-to-many Relationship for Veilingmeester veilingklokken
            modelBuilder.Entity<VeilingKlok>()
                .HasOne(vk => vk.Veilingmeester)
                .WithMany(vm => vm.Veilingklokken)
                .HasForeignKey(vk => vk.VeilingmeesterId)
                .IsRequired();

            // 6. One-to-many Relationship for Kweker products
            modelBuilder.Entity<Product>()
                .HasOne(pd => pd.Kweker)
                .WithMany(kw => kw.Products)
                .HasForeignKey(pd => pd.KwekerId)
                .IsRequired();

            // 7. One-to-many Relationship for Veilingklok products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.VeilingKlok)
                .WithMany(vk => vk.Products)
                .HasForeignKey(p => p.VeilingKlokId)
                .IsRequired();
            modelBuilder.Entity<Product>().HasOne(pd => pd.Kweker).WithMany(kw => kw.Products)
                .HasForeignKey(pd => pd.KwekerId).IsRequired();

            // 6. One-to-many Relationship for Account to RefreshTokens
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // Delete tokens when account is deleted
        }
    }
}
