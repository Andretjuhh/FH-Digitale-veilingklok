using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Address> Addresses { get; set; }
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

        #region GLOBAL DEFAULTS (IDs & Timestamps)

        modelBuilder.Entity<Account>().Property(a => a.Id).HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<Order>().Property(o => o.Id).HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<Product>().Property(p => p.Id).HasDefaultValueSql("NEWID()");

        modelBuilder.Entity<VeilingKlok>().Property(vk => vk.Id).HasDefaultValueSql("NEWID()");

        modelBuilder
            .Entity<Account>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder
            .Entity<Order>()
            .Property(o => o.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder
            .Entity<OrderItem>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder
            .Entity<Product>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder
            .Entity<VeilingKlok>()
            .Property(vk => vk.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder
            .Entity<RefreshToken>()
            .Property(rt => rt.CreatedAt)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder.Entity<Account>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Order>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<OrderItem>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<VeilingKlok>().Property(p => p.RowVersion).IsRowVersion();

        #endregion

        #region ACCOUNT HIERARCHY & AUTHENTICATION

        // Password Value Object Conversion
        modelBuilder
            .Entity<Account>()
            .Property(a => a.Password)
            .HasConversion(password => password.Value, hash => Password.FromHash(hash))
            .HasColumnName("password");

        // Table-Per-Type (TPT) Strategy
        modelBuilder.Entity<Account>().UseTptMappingStrategy();

        // Unique Email Index
        modelBuilder.Entity<Account>().HasIndex(a => a.Email).IsUnique();

        // RefreshToken Configuration
        modelBuilder.Entity<RefreshToken>().HasIndex(rt => rt.Jti);
        modelBuilder.Entity<RefreshToken>().HasIndex(rt => rt.AccountId);

        // Account -> RefreshTokens (Cascade Delete)
        // Configure from Account side to properly use the navigation property
        modelBuilder
            .Entity<Account>()
            .HasMany(a => a.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region ADDRESS CONFIGURATION

        // Address -> Account (base class for Koper and Kweker)
        // This allows addresses to be linked to any Account type
        // Using Restrict to avoid multiple cascade paths with PrimaryAdressId
        modelBuilder
            .Entity<Address>()
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(a => a.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region KOPER (BUYER) CONFIGURATION

        // Koper -> Primary Address (One-to-One, Restrict Delete)
        modelBuilder
            .Entity<Koper>()
            .HasOne<Address>()
            .WithMany()
            .HasForeignKey(k => k.PrimaryAdressId)
            .IsRequired(false) // Make it optional so it can be null initially
            .OnDelete(DeleteBehavior.Restrict);
        // you cannot delete an Address if it's referenced as a PrimaryAdressId by any Koper.

        // Koper -> Orders (Restrict Delete)
        modelBuilder
            .Entity<Koper>()
            .HasMany<Order>()
            .WithOne()
            .HasForeignKey(o => o.KoperId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region KWEKER (GROWER) CONFIGURATION

        // Unique KvkNumber Index
        modelBuilder.Entity<Kweker>().HasIndex(k => k.KvkNumber).IsUnique();

        // Kweker -> Address (Main Address, Restrict Delete)
        modelBuilder
            .Entity<Kweker>()
            .HasOne(k => k.Adress)
            .WithMany()
            .HasForeignKey(k => k.AdressId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Kweker -> Products (Restrict Delete)
        modelBuilder
            .Entity<Kweker>()
            .HasMany<Product>()
            .WithOne()
            .HasForeignKey(p => p.KwekerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        #endregion

        #region VEILINGMEESTER (AUCTIONEER) CONFIGURATION

        // Veilingmeester -> VeilingKlok (Restrict Delete)
        modelBuilder
            .Entity<Veilingmeester>()
            .HasMany<VeilingKlok>()
            .WithOne()
            .HasForeignKey(vk => vk.VeilingmeesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Decimal precision for VeilingKlok prices
        modelBuilder.Entity<VeilingKlok>().Property(vk => vk.HighestPrice).HasPrecision(18, 2);
        modelBuilder.Entity<VeilingKlok>().Property(vk => vk.LowestPrice).HasPrecision(18, 2);

        #endregion

        #region PRODUCT & INVENTORY

        // Decimal precision for Product prices
        modelBuilder.Entity<Product>().Property(p => p.AuctionPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.MinimumPrice).HasPrecision(18, 2);

        // Check Constraint: MinimumPrice <= AuctionPrice (when AuctionPrice is not null)
        modelBuilder
            .Entity<Product>()
            .ToTable(tb =>
                tb.HasCheckConstraint(
                    "CK_Product_MinimumPrice",
                    "[auction_price] IS NULL OR [minimum_price] <= [auction_price]"
                )
            );

        // Product -> VeilingKlok (Optional, Restrict Delete)
        modelBuilder
            .Entity<Product>()
            .HasOne<VeilingKlok>()
            .WithMany()
            .HasForeignKey(p => p.VeilingKlokId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false); // A  Product may or may not be associated with a VeilingKlok.

        // Indexes to speed up price history queries
        modelBuilder.Entity<OrderItem>().HasIndex(oi => oi.CreatedAt);
        modelBuilder.Entity<OrderItem>().HasIndex(oi => new { oi.ProductId, oi.CreatedAt });

        // OrderItem -> Product (Restrict Delete)
        modelBuilder
            .Entity<Product>()
            .HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder
            .Entity<VeilingKlok>()
            .HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey(oi => oi.VeilingKlokId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        #endregion

        #region ORDER MANAGEMENT

        // Order -> OrderItems (Cascade Delete)
        modelBuilder
            .Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder
            .Entity<Order>()
            .HasOne<VeilingKlok>()
            .WithMany()
            .HasForeignKey(o => o.VeilingKlokId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        #endregion
    }
}
