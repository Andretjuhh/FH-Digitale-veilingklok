using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Models.Domain;

namespace VeilingKlokApp.Data
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Koper> Kopers { get; set; }
        public DbSet<Kweker> Kwekers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Veilingmeester> Veilingmeesters { get; set; }
        public DbSet<VeilingKlok> Veilingklokken { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //
            // 0. Auto-generate GUIDs for PKs and timestamps inside the database
            //
            modelBuilder.Entity<Account>().Property(a => a.Id).HasDefaultValueSql("NEWID()");
            modelBuilder
                .Entity<Account>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Order>().Property(o => o.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Product>().Property(p => p.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<VeilingKlok>().Property(vk => vk.Id).HasDefaultValueSql("NEWID()");

            modelBuilder
                .Entity<RefreshToken>()
                .Property(rt => rt.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            //
            // 1. Table-Per-Type (TPT)
            //
            modelBuilder.Entity<Account>().UseTptMappingStrategy();

            //
            // 2. Unique Email index
            //
            modelBuilder.Entity<Account>().HasIndex(a => a.Email).IsUnique();

            //
            // 2.A. Unique KvkNumber index for Kweker
            //
            modelBuilder.Entity<Kweker>().HasIndex(k => k.KvkNumber).IsUnique();

            //
            // 3. Koper -> Orders (one definition, safe delete behavior)
            //
            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.Koper)
                .WithMany(k => k.Orders)
                .HasForeignKey(o => o.KoperId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 3.A. Order -> OrderItems (cascade delete)
            //
            modelBuilder
                .Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            //
            // 3.B. Product -> OrderItems (restrict delete)
            //
            modelBuilder
                .Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 4. Veilingmeester -> VeilingKlok
            //
            modelBuilder
                .Entity<VeilingKlok>()
                .HasOne(vk => vk.Veilingmeester)
                .WithMany(vm => vm.Veilingklokken)
                .HasForeignKey(vk => vk.VeilingmeesterId)
                .OnDelete(DeleteBehavior.Restrict) // IMPORTANT FIX
                .IsRequired();

            //
            // 5. Kweker -> Products
            //
            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.Kweker)
                .WithMany(k => k.Products)
                .HasForeignKey(p => p.KwekerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //
            // 5.A Product - Add CHECK constraint for minimum_price <= price
            //
            modelBuilder
                .Entity<Product>()
                .ToTable(tb =>
                    tb.HasCheckConstraint("CK_Product_MinimumPrice", "[minimum_price] <= [price]")
                );

            //
            // 6. VeilingKlok -> Products (product may exist without being assigned to a VeilingKlok)
            //
            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.VeilingKlok)
                .WithMany(vk => vk.Products)
                .HasForeignKey(p => p.VeilingKlokId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // make the FK optional

            //
            // 7. Account -> RefreshTokens (Cascade allowed)
            //
            modelBuilder
                .Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
