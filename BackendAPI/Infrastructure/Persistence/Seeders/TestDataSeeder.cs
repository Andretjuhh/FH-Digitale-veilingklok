using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders;

public interface ITestDataSeeder
{
    Task SeedAsync();
    Task ClearAllDataAsync();
}

public class TestDataSeeder : ITestDataSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<TestDataSeeder> _logger;

    // Default Password is : Test1234!
    // Email format:  koper{i + 1}@test.nl || kweker{i + 1}@test.nl || veilingmeester{i + 1}@test.nl

    // Dutch flower and plant names
    private static readonly string[] DutchFlowerNames = new[]
    {
        "Tulpen",
        "Rozen",
        "Narcissen",
        "Hyacinten",
        "Lelies",
        "Gerbera's",
        "Chrysanten",
        "Orchideeën",
        "Anjers",
        "Fresia's",
        "Amaryllis",
        "Pioenen",
        "Dahlia's",
        "Irissen",
        "Gladiolen",
        "Alstroemeria",
        "Zonnebloemen",
        "Lisianthus",
        "Ranonkels",
        "Anemonen"
    };

    private static readonly string[] DutchColors = new[]
    {
        "Rood",
        "Roze",
        "Wit",
        "Geel",
        "Oranje",
        "Paars",
        "Blauw",
        "Gemengd"
    };

    private static readonly string[] FlowerDescriptions = new[]
    {
        "Verse snijbloemen direct van de kweker",
        "Premium kwaliteit voor groothandel",
        "Seizoensgebonden bloemen",
        "Exportkwaliteit met certificaat",
        "Biologisch geteelde bloemen",
        "Langbloeiend en sterk",
        "Perfect voor boeketten",
        "Ideaal voor evenementen"
    };

    // Dutch cities for addresses
    private static readonly (string City, string PostalCode, string Region)[] DutchLocations = new[]
    {
        ("Amsterdam", "1012", "Noord-Holland"),
        ("Rotterdam", "3011", "Zuid-Holland"),
        ("Utrecht", "3511", "Utrecht"),
        ("Eindhoven", "5611", "Noord-Brabant"),
        ("Groningen", "9711", "Groningen"),
        ("Tilburg", "5011", "Noord-Brabant"),
        ("Almere", "1315", "Flevoland"),
        ("Breda", "4811", "Noord-Brabant"),
        ("Nijmegen", "6511", "Gelderland"),
        ("Haarlem", "2011", "Noord-Holland"),
        ("Arnhem", "6811", "Gelderland"),
        ("Zaandam", "1506", "Noord-Holland"),
        ("Leiden", "2311", "Zuid-Holland"),
        ("Amersfoort", "3811", "Utrecht"),
        ("Apeldoorn", "7311", "Gelderland")
    };

    private static readonly string[] DutchStreets = new[]
    {
        "Kerkstraat",
        "Hoofdstraat",
        "Dorpsstraat",
        "Schoolstraat",
        "Molenstraat",
        "Marktplein",
        "Stationsweg",
        "Parkweg",
        "Nieuwstraat",
        "Lange Gracht"
    };

    private static readonly string[] CompanyNames = new[]
    {
        "Bloemenweelde BV",
        "Flora Export Nederland",
        "De Groene Kwekerij",
        "Holland Flowers Groothandel",
        "Tulp & Co",
        "Nederlandse Bloemenveiling",
        "Kwekerij Van der Berg",
        "Floral Trade Amsterdam",
        "Bloemendaal Kwekers",
        "Royal Dutch Flowers",
        "Aalsmeer Bloemen BV",
        "Green Garden Holland"
    };

    private static readonly string[] FirstNames = new[]
    {
        "Jan",
        "Pieter",
        "Kees",
        "Hans",
        "Willem",
        "Hendrik",
        "Dirk",
        "Maarten",
        "Anna",
        "Maria",
        "Sophie",
        "Emma",
        "Lisa",
        "Eva",
        "Sara",
        "Julia"
    };

    private static readonly string[] LastNames = new[]
    {
        "de Jong",
        "Jansen",
        "de Vries",
        "van den Berg",
        "van Dijk",
        "Bakker",
        "Visser",
        "Smit",
        "Meijer",
        "de Boer",
        "Mulder",
        "de Groot"
    };

    public TestDataSeeder(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<TestDataSeeder> logger
    )
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting test data seeding...");

            // Clear existing data first
            await ClearAllDataAsync();

            await _context.Database.BeginTransactionAsync();
            // Seed in order of dependencies
            var kwekers = await SeedKwekersAsync(5);
            var kopers = await SeedKopersAsync(10);
            var veilingmeesters = await SeedVeilingmeestersAsync(3);
            var products = await SeedProductsAsync(kwekers);
            var veilingklokken = await SeedVeilingKlokkenAsync(veilingmeesters, products);
            await SeedOrdersAsync(kopers, veilingklokken, products);

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            _logger.LogInformation("Test data seeding completed successfully!");
            _logger.LogInformation(
                $"Created: {kwekers.Count} kwekers, {kopers.Count} kopers, "
                + $"{veilingmeesters.Count} veilingmeesters, {products.Count} products, "
                + $"{veilingklokken.Count} veilingklokken"
            );
        }
        catch (Exception ex)
        {
            await _context.Database.RollbackTransactionAsync();
            _logger.LogError(ex, "Error during test data seeding");
            throw;
        }
    }

    public async Task ClearAllDataAsync()
    {
        _logger.LogInformation("Clearing existing test data...");

        // Delete in reverse order of dependencies
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [OrderItem]");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Order]");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Product]");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Veilingklok]");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [RefreshToken]");
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Veilingmeester]");
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM [Kweker]"); // Delete Kweker before Adresses (has FK to Adresses)
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM [Koper]"); // Delete Koper before Adresses (has FK to Adresses)
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Adresses]"); // Delete Adresses after Kweker and Koper
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Account]");

        await _context.SaveChangesAsync();
        _logger.LogInformation("Existing data cleared.");
    }

    private async Task<List<Kweker>> SeedKwekersAsync(int count)
    {
        var kwekers = new List<Kweker>();
        var random = new Random(42); // Fixed seed for reproducibility

        for (var i = 0; i < count; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var password = Password.Create("Test1234!", _passwordHasher);

            var kweker = new Kweker($"kweker{i + 1}@test.nl", password)
            {
                KvkNumber = $"{60000000 + i + random.Next(1000, 9999)}",
                CompanyName = CompanyNames[i % CompanyNames.Length],
                FirstName = firstName,
                LastName = lastName,
                Telephone = $"+31 6 {random.Next(10000000, 99999999)}"
            };

            await _context.Kwekers.AddAsync(kweker);
            await _context.SaveChangesAsync(); // Save to get Kweker ID
            kwekers.Add(kweker);

            var location = DutchLocations[i % DutchLocations.Length];
            var address = new Address(
                $"{DutchStreets[random.Next(DutchStreets.Length)]} {random.Next(1, 200)}",
                location.City,
                location.Region,
                $"{location.PostalCode} {(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}",
                "NL",
                kweker.Id
            );

            kweker.UpdateAdress(address);
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync(); // Save to get address ID
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {count} kwekers");
        return kwekers;
    }

    private async Task<List<Koper>> SeedKopersAsync(int count)
    {
        var kopers = new List<Koper>();
        var random = new Random(43);

        for (var i = 0; i < count; i++)
        {
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var password = Password.Create("Test1234!", _passwordHasher);

            var koper = new Koper($"koper{i + 1}@test.nl", password)
            {
                FirstName = firstName,
                LastName = lastName,
                Telephone = $"+31 6 {random.Next(10000000, 99999999)}"
            };

            await _context.Kopers.AddAsync(koper);
            await _context.SaveChangesAsync(); // Save to get koper ID

            // Add 1-3 addresses per koper
            var addressCount = random.Next(1, 4);
            Address? primaryAddress = null;

            for (var j = 0; j < addressCount; j++)
            {
                var location = DutchLocations[random.Next(DutchLocations.Length)];
                var address = new Address(
                    $"{DutchStreets[random.Next(DutchStreets.Length)]} {random.Next(1, 200)}",
                    location.City,
                    location.Region,
                    $"{location.PostalCode} {(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}",
                    "NL",
                    koper.Id
                );

                // Add address WITHOUT setting as primary yet (address needs ID first)
                koper.AddNewAdress(address, false);

                // Remember the first address to set as primary later
                if (j == 0)
                    primaryAddress = address;
            }

            // Save all addresses to get their IDs
            await _context.SaveChangesAsync();

            // Now set the primary address (after addresses have IDs)
            if (primaryAddress != null)
            {
                koper.SetPrimaryAdress(primaryAddress);
                await _context.SaveChangesAsync();
            }

            kopers.Add(koper);
        }

        _logger.LogInformation($"Seeded {count} kopers");
        return kopers;
    }

    private async Task<List<Veilingmeester>> SeedVeilingmeestersAsync(int count)
    {
        var veilingmeesters = new List<Veilingmeester>();
        var random = new Random(44);
        var regions = new[]
        {
            "Noord-Holland",
            "Zuid-Holland",
            "Utrecht",
            "Noord-Brabant",
            "Gelderland"
        };

        for (var i = 0; i < count; i++)
        {
            var password = Password.Create("Test1234!", _passwordHasher);

            var veilingmeester = new Veilingmeester($"veilingmeester{i + 1}@test.nl", password)
            {
                CountryCode = "NL",
                Region = regions[i % regions.Length],
                AuthorisatieCode = $"VM{1000 + i}-NL"
            };

            await _context.Veilingmeesters.AddAsync(veilingmeester);
            veilingmeesters.Add(veilingmeester);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {count} veilingmeesters");
        return veilingmeesters;
    }

    private async Task<List<Product>> SeedProductsAsync(List<Kweker> kwekers)
    {
        var products = new List<Product>();
        var random = new Random(45);

        foreach (var kweker in kwekers)
            // Create 20 products per kweker
            for (var i = 0; i < 20; i++)
            {
                var flowerName = DutchFlowerNames[random.Next(DutchFlowerNames.Length)];
                var color = DutchColors[random.Next(DutchColors.Length)];
                var description = FlowerDescriptions[random.Next(FlowerDescriptions.Length)];
                var minimumPrice = Math.Round((decimal)(random.NextDouble() * 15 + 5), 2); // €5-€20
                var stock = random.Next(50, 500);

                var product = new Product(minimumPrice, stock)
                {
                    Name = $"{color} {flowerName}",
                    Description = description,
                    ImageUrl = GenerateBase64PlaceholderImage(),
                    Dimension = $"{random.Next(30, 80)}cm",
                    KwekerId = kweker.Id
                };

                await _context.Products.AddAsync(product);
                products.Add(product);
            }

        await _context.SaveChangesAsync();
        _logger.LogInformation(
            $"Seeded {products.Count} products ({kwekers.Count} kwekers × 20 products)"
        );
        return products;
    }

    private async Task<List<VeilingKlok>> SeedVeilingKlokkenAsync(
        List<Veilingmeester> veilingmeesters,
        List<Product> allProducts
    )
    {
        var veilingklokken = new List<VeilingKlok>();
        var random = new Random(46);
        var regions = new[]
        {
            "Noord-Holland",
            "Zuid-Holland",
            "Utrecht",
            "Noord-Brabant",
            "Gelderland"
        };

        // Create 10 veilingklokken
        for (var i = 0; i < 10; i++)
        {
            var veilingmeester = veilingmeesters[i % veilingmeesters.Count];
            var location = DutchLocations[random.Next(DutchLocations.Length)];

            var veilingKlok = new VeilingKlok
            {
                VeilingDurationMinutes = random.Next(60, 180),
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(random.Next(-5, 30)),
                RegionOrState = location.Region,
                Country = "NL"
            };

            // Assign veilingmeester using reflection since the setter is private
            typeof(VeilingKlok)
                .GetProperty("VeilingmeesterId")
                ?.SetValue(veilingKlok, veilingmeester.Id);

            await _context.Veilingklokken.AddAsync(veilingKlok);
            await _context.SaveChangesAsync(); // Save to get ID

            // 70% chance to add products (30% empty klokken)
            if (random.NextDouble() > 0.3)
            {
                // Add 5-15 random products
                var productCount = random.Next(5, 16);
                var selectedProducts = allProducts
                    .OrderBy(x => random.Next())
                    .Take(productCount)
                    .ToList();

                var lowestPrice = decimal.MaxValue;
                var highestPrice = decimal.MinValue;

                foreach (var product in selectedProducts)
                {
                    product.AddToVeilingKlok(veilingKlok.Id);

                    if (product.MinimumPrice < lowestPrice)
                        lowestPrice = product.MinimumPrice;
                    if (product.MinimumPrice > highestPrice)
                        highestPrice = product.MinimumPrice;
                }

                veilingKlok.LowestPrice = lowestPrice;
                veilingKlok.HighestPrice = highestPrice;
            }

            veilingklokken.Add(veilingKlok);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {veilingklokken.Count} veilingklokken");
        return veilingklokken;
    }

    private async Task SeedOrdersAsync(
        List<Koper> kopers,
        List<VeilingKlok> veilingklokken,
        List<Product> products
    )
    {
        var random = new Random(47);
        var ordersCreated = 0;

        // Create some orders for random kopers
        foreach (var koper in kopers.Take(5)) // First 5 kopers get orders
        {
            var veilingKlok = veilingklokken[random.Next(veilingklokken.Count)];

            var order = new Order(koper.Id) { VeilingKlokId = veilingKlok.Id };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); // Save to get order ID

            // Add 2-5 order items
            var itemCount = random.Next(2, 6);
            var selectedProducts = products
                .Where(p => p.VeilingKlokId == veilingKlok.Id)
                .OrderBy(x => random.Next())
                .Take(itemCount)
                .ToList();

            foreach (var product in selectedProducts)
            {
                var quantity = random.Next(1, 10);
                // Ensure price is at least the minimum price, or slightly higher if auctioned
                var unitPrice = product.AuctionPrice ?? product.MinimumPrice;
                // If auction price is set, use it; otherwise add 10-50% markup to minimum price
                if (!product.AuctionPrice.HasValue)
                    unitPrice =
                        product.MinimumPrice * (1 + (decimal)(random.NextDouble() * 0.4 + 0.1));

                // Fetch the full product object for the constructor
                var fullProduct = await _context.Products.FindAsync(product.Id);
                if (fullProduct == null)
                    continue;

                var orderItem = new OrderItem(unitPrice, quantity, fullProduct, order.Id)
                {
                    VeilingKlokId = veilingKlok.Id
                };

                order.AddItem(orderItem);
                await _context.OrderItems.AddAsync(orderItem);
            }

            ordersCreated++;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {ordersCreated} orders with items");
    }

    private string GenerateBase64PlaceholderImage()
    {
        // Simple 1x1 pixel transparent PNG in base64
        return "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
    }
}