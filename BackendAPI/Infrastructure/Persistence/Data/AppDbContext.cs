using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<Account, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Koper> Kopers { get; set; }
    public DbSet<Kweker> Kwekers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Veilingmeester> Veilingmeesters { get; set; }
    public DbSet<VeilingKlok> Veilingklokken { get; set; }
    public DbSet<VeilingKlokProduct> VeilingKlokProducts { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region GLOBAL DEFAULTS (IDs & Timestamps)

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

        // Account RowVersion - IdentityUser has ConcurrencyStamp, but we kept RowVersion in Account.cs
        modelBuilder.Entity<Account>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Order>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<OrderItem>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<VeilingKlok>().Property(p => p.RowVersion).IsRowVersion();

        #endregion

        #region ACCOUNT HIERARCHY & AUTHENTICATION

        // Table-Per-Type (TPT) Strategy
        modelBuilder.Entity<Account>().UseTptMappingStrategy();

        // Map Identity Tables to clean names if desired, but sticking to defaults for Identity tables is standard.

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

        // Decimal precision for VeilingKlokProduct prices
        modelBuilder
            .Entity<VeilingKlokProduct>()
            .Property(vk => vk.AuctionPrice)
            .HasPrecision(18, 2);

        // VeilingKlok -> VeilingKlokProducts (Cascade Delete)
        modelBuilder
            .Entity<VeilingKlok>()
            .HasMany(vk => vk.VeilingKlokProducts)
            .WithOne()
            .HasForeignKey(vkp => vkp.VeilingKlokId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        #endregion

        #region PRODUCT & INVENTORY

        // Indexes for performance optimization on filtering and sorting
        modelBuilder.Entity<Product>().HasIndex(p => p.CreatedAt);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name);
        modelBuilder.Entity<Product>().HasIndex(p => p.Region);
        modelBuilder.Entity<Product>().HasIndex(p => p.AuctionPrice);
        modelBuilder.Entity<Product>().HasIndex(p => p.KwekerId);
        modelBuilder.Entity<Product>().HasIndex(p => p.VeilingKlokId);

        // VeilingKlok Indexes
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.Status);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.ScheduledAt);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.StartedAt);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.EndedAt);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.VeilingmeesterId);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.RegionOrState);
        modelBuilder.Entity<VeilingKlok>().HasIndex(vk => vk.Country);

        // Decimal precision for Product prices
        modelBuilder.Entity<Product>().Property(p => p.AuctionPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.MinimumPrice).HasPrecision(18, 2);

        // Decimal precision for OrderItem price
        modelBuilder.Entity<OrderItem>().Property(oi => oi.PriceAtPurchase).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(oi => oi.ProductMinimumPrice).HasPrecision(18, 2);

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

        // VeilingKlokProduct -> Product (Restrict Delete)
        modelBuilder
            .Entity<VeilingKlokProduct>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(vkp => vkp.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

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

        // Indexes for performance optimization
        modelBuilder.Entity<Order>().HasIndex(o => o.KoperId);
        modelBuilder.Entity<Order>().HasIndex(o => o.CreatedAt);
        modelBuilder.Entity<Order>().HasIndex(o => o.Status);
        modelBuilder.Entity<Order>().HasIndex(o => o.VeilingKlokId);
        modelBuilder.Entity<OrderItem>().HasIndex(oi => oi.OrderId);
        modelBuilder.Entity<OrderItem>().HasIndex(oi => oi.ProductId);

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
