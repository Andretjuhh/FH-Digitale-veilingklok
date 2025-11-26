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
            base.OnModelCreating(modelBuilder);

            //
            // 1. Table-Per-Type (TPT)
            //
            modelBuilder.Entity<Account>()
                .UseTptMappingStrategy();

            //
            // 2. Unique Email index
            //
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            //
            // 3. Koper -> Orders (one definition, safe delete behavior)
            //
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Koper)
                .WithMany(k => k.Orders)
                .HasForeignKey(o => o.KoperId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 4. Veilingmeester -> VeilingKlok
            //
            modelBuilder.Entity<VeilingKlok>()
                .HasOne(vk => vk.Veilingmeester)
                .WithMany(vm => vm.Veilingklokken)
                .HasForeignKey(vk => vk.VeilingmeesterId)
                .OnDelete(DeleteBehavior.Restrict)   // IMPORTANT FIX
                .IsRequired();

            //
            // 5. Kweker -> Products
            //
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Kweker)
                .WithMany(k => k.Products)
                .HasForeignKey(p => p.KwekerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 6. VeilingKlok -> Products
            //
            modelBuilder.Entity<Product>()
                .HasOne(p => p.VeilingKlok)
                .WithMany(vk => vk.Products)
                .HasForeignKey(p => p.VeilingKlokId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 7. Account -> RefreshTokens (Cascade allowed)
            //
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
