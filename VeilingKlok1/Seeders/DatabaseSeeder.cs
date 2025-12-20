using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Services;

namespace VeilingKlokApp.Data
{
    /// <summary>
    /// Simple database seeder to add dummy data for development/testing.
    /// Call SeedAsync() at startup in development only.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly DatabaseContext _db;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(DatabaseContext db, IPasswordHasher passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            // Apply pending migrations (optional)
            try
            {
                await _db.Database.MigrateAsync();
            }
            catch
            {
                // If migrations are not configured or DB not reachable, continue with inserts if possible
            }

            // Seed Kwekers
            if (!await _db.Kwekers.AnyAsync())
            {
                var kweker1 = new Kweker
                {
                    Email = "kweker1@example.com",
                    Password = _passwordHasher.HashPassword("KwekerPass1!"),
                    Name = "Bloemen Kwekerij",
                    Telephone = "+31101234567",
                    Adress = "Kwekerstraat 1",
                    PostCode = "1234 AB",
                    Regio = "Randstad",
                    KvkNumber = "NL123456789B01",
                    CreatedAt = DateTime.UtcNow,
                };

                var kweker2 = new Kweker
                {
                    Email = "kweker2@example.com",
                    Password = _passwordHasher.HashPassword("KwekerPass2!"),
                    Name = "Planten Kwekerij",
                    Telephone = "+31107654321",
                    Adress = "Plantenlaan 2",
                    PostCode = "5678 CD",
                    Regio = "Noord-Holland",
                    KvkNumber = "NL987654321B02",
                    CreatedAt = DateTime.UtcNow,
                };

                _db.Kwekers.AddRange(kweker1, kweker2);
                await _db.SaveChangesAsync();

                // Seed multiple Products (20 examples) distributed across kweker1 and kweker2
                var products = Enumerable
                    .Range(1, 20)
                    .Select(i =>
                    {
                        // Alternate between kweker1 and kweker2
                        var ownerId = (i % 2 == 0) ? kweker2.Id : kweker1.Id;
                        var basePrice = 5.00m + i; // simple price increment
                        var sizes = new[] { "XS", "S", "M", "L", "XL" };
                        return new Product
                        {
                            Name = $"Product {i} - Voorbeeld",
                            Description = $"Dummy product #{i} voor ontwikkelomgeving",
                            Price = Math.Round(basePrice, 2),
                            MinimumPrice = Math.Round(basePrice * 0.5m, 2),
                            Stock = 50 + i * 10,
                            ImageUrl = null,
                            Size = sizes[i % sizes.Length],
                            KwekerId = ownerId,
                        };
                    })
                    .ToList();

                _db.Products.AddRange(products);
                await _db.SaveChangesAsync();
            }

            // Seed Kopers
            if (!await _db.Kopers.AnyAsync())
            {
                var koper1 = new Koper
                {
                    Email = "koper1@example.com",
                    Password = _passwordHasher.HashPassword("KoperPass1!"),
                    FirstName = "Jan",
                    LastName = "Jansen",
                    Telephone = "+31105556666",
                    Adress = "Koperstraat 3",
                    PostCode = "1234 AB",
                    Regio = "Randstad",
                    CreatedAt = DateTime.UtcNow,
                };

                var koper2 = new Koper
                {
                    Email = "koper2@example.com",
                    Password = _passwordHasher.HashPassword("KoperPass2!"),
                    FirstName = "Piet",
                    LastName = "Pietersen",
                    Telephone = "+31107778888",
                    Adress = "Markt 4",
                    PostCode = "5678 CD",
                    Regio = "Noord-Brabant",
                    CreatedAt = DateTime.UtcNow,
                };

                _db.Kopers.AddRange(koper1, koper2);
                await _db.SaveChangesAsync();
            }

            // Seed Veilingklokken
            if (!await _db.Veilingklokken.AnyAsync())
            {
                // ensure there is a Veilingmeester to own the clock
                var veilingmeester = await _db.Veilingmeesters.FirstOrDefaultAsync();
                if (veilingmeester == null)
                {
                    veilingmeester = new Veilingmeester
                    {
                        Email = "veilingmeester@example.com",
                        Password = _passwordHasher.HashPassword("VeilingPass1!"),
                        Regio = "Landelijk",
                        AuthorisatieCode = "AUTH-12345",
                        CreatedAt = DateTime.UtcNow,
                    };
                    _db.Veilingmeesters.Add(veilingmeester);
                    await _db.SaveChangesAsync();
                }

                var now = DateTime.UtcNow;
                var klok1 = new VeilingKlok
                {
                    Naam = "Herfstveiling",
                    DurationInSeconds = 3600,
                    LiveViews = 0,
                    StartTime = now.AddHours(1),
                    EndTime = now.AddHours(2),
                    VeilingmeesterId = veilingmeester.Id,
                };

                _db.Veilingklokken.Add(klok1);
                await _db.SaveChangesAsync();
            }
        }
    }
}
