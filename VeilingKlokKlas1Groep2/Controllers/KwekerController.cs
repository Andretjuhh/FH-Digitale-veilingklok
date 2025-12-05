using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;

using VeilingKlokApp.Data;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;
using VeilingKlokKlas1Groep2.Services;
using VeilingKlokKlas1Groep2.Configuration;
using VeilingKlokKlas1Groep2.Attributes;

namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for managing Kweker (Grower) operations
    /// Handles CRUD operations for grower accounts and their products
    /// </summary>
    [ApiController]
    [Route("api/kweker")]
    public class KwekerController : ControllerBase
    {
        private readonly VeilingKlokContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public KwekerController(VeilingKlokContext db, IPasswordHasher passwordHasher, IAuthService authService)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        #region Create Kweker Account

        /// <summary>
        /// Creates a new Kweker (Grower) account
        /// </summary>
        /// <param name="newKweker">Kweker account details to create</param>
        /// <returns>Created account ID and tokens with HTTP 200 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateKwekerAccount([FromBody] NewKwekerAccount newKweker)
        {
            // Validate input payload
            if (newKweker == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Kweker account data is required",
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Check for existing account by email using LINQ
                // Provides better error message than DB unique constraint
                if (await _db.Accounts.AnyAsync(a => a.Email == newKweker.Email))
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Conflict",
                        "An account with this email already exists",
                        409
                    );
                    return Conflict(error);
                }

                // Create Kweker entity (Account is abstract base class)
                var kweker = new Kweker
                {
                    Email = newKweker.Email,
                    Password = _passwordHasher.HashPassword(newKweker.Password),
                    CreatedAt = DateTime.UtcNow,
                    Name = newKweker.Name,
                    Telephone = newKweker.Telephone,
                    Adress = newKweker.Adress,
                    Regio = newKweker.Regio,
                    KvkNumber = newKweker.KvkNumber
                };

                _db.Kwekers.Add(kweker);
                await _db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var accountType = "Kweker";
                var authResponse = await _authService.SignInAsync(kweker, accountType, Response);

                await transaction.CommitAsync();

                return Ok(authResponse);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Database Error",
                    $"Database constraint violation: {ex.InnerException?.Message ?? ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Internal Server Error",
                    $"Account creation failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get Kweker Stats

        /// <summary>
        /// Returns aggregated statistics for the authenticated Kweker
        /// </summary>
        [HttpGet("stats")]
        [Authorize]
        [AuthorizeAccountType("Kweker")]
        public async Task<IActionResult> GetKwekerStats()
        {
            try
            {
                var accountId = HttpContext.Items["AccountId"] as int?;
                if (!accountId.HasValue)
                {
                    var error = new HtppError("Unauthorized", "Invalid token claims", 401);
                    return Unauthorized(error);
                }

                // Total products for this kweker
                var totalProducts = await _db.Products.Where(p => p.KwekerId == accountId.Value).CountAsync();

                // Active auctions: products that are currently assigned to a VeilingKlok (simple heuristic)
                var activeAuctions = await _db.Products.Where(p => p.KwekerId == accountId.Value && p.VeilingKlokId != null).CountAsync();

                // Stems sold and total revenue: aggregate over Orders joined with Products
                var orderQuery = _db.Orders
                    .Where(o => o.Product.KwekerId == accountId.Value)
                    .AsQueryable();

                var stemsSold = 0;
                decimal totalRevenue = 0m;

                if (await orderQuery.AnyAsync())
                {
                    stemsSold = await orderQuery.SumAsync(o => o.Quantity);

                    // Sum revenue using the current Product.Price (approximation)
                    totalRevenue = await orderQuery
                        .Select(o => new { o.Quantity, ProductPrice = o.Product.Price })
                        .SumAsync(x => x.Quantity * x.ProductPrice);
                }

                return Ok(new
                {
                    totalProducts,
                    activeAuctions,
                    totalRevenue,
                    stemsSold
                });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Kweker stats: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get Authenticated Kweker

        /// <summary>
        /// Returns the authenticated Kweker's account details (current user)
        /// Requires the user to be authenticated and of account type 'Kweker'
        /// </summary>
        [HttpGet("account-info")]
        [Authorize]
        [AuthorizeAccountType("Kweker")]
        public async Task<IActionResult> GetCurrentKweker()
        {
            try
            {
                var accountId = HttpContext.Items["AccountId"] as int?;

                if (!accountId.HasValue)
                {
                    var error = new HtppError("Unauthorized", "Invalid token claims", 401 );
                    return Unauthorized(error);
                }

                var kwekerDetails = await _db.Kwekers
                    .Where(k => k.Id == accountId.Value)
                    .Select(k => new KwekerDetails
                    {
                        AccountId = k.Id,
                        Name = k.Name,
                        Email = k.Email,
                        Telephone = k.Telephone,
                        Adress = k.Adress,
                        Regio = k.Regio,
                        KvkNumber = k.KvkNumber,
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (kwekerDetails == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        "Authenticated Kweker account not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(kwekerDetails);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve current Kweker account: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get Authenticated Kweker products

        /// <summary>
        /// Returns all products for the authenticated Kweker (current user)
        /// Protected: Requires authentication and account type 'Kweker'
        /// </summary>
        [HttpGet("products")]
        [Authorize]
        [AuthorizeAccountType("Kweker")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var accountId = HttpContext.Items["AccountId"] as int?;
                if (!accountId.HasValue)
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid token claims",
                        401
                    );
                    return Unauthorized(error);
                }

                var products = await _db.Products
                    .Where(p => p.KwekerId == accountId.Value)
                    .Select(p => new ProductDetails
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        MinimumPrice = p.MinimumPrice,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        Size = p.Size,
                        KwekerId = p.KwekerId
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new { products });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve products for authenticated Kweker: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region (Admin) Get Kweker Account

        /// <summary>
        /// Retrieves a Kweker account by ID
        /// </summary>
        /// <param name="accountId">The ID of the Kweker account to retrieve</param>
        /// <returns>Kweker account details with HTTP 200 status</returns>
        [HttpGet("admin/{accountId}")]
        public async Task<ActionResult<KwekerDetails>> GetKwekerAccount(int accountId)
        {
            try
            {
                // Retrieve Kweker using LINQ with projection to DTO
                var kwekerDetails = await _db.Kwekers
                    .Where(k => k.Id == accountId)
                    .Select(k => new KwekerDetails
                    {
                        AccountId = k.Id,
                        Name = k.Name,
                        Email = k.Email,     // Base property from Account
                        Telephone = k.Telephone,
                        Adress = k.Adress,
                        Regio = k.Regio,
                        KvkNumber = k.KvkNumber,
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (kwekerDetails == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Kweker account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(kwekerDetails);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Kweker account: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region (Admin)  Get Kweker Products

        /// <summary>
        /// Retrieves all products for a specific Kweker
        /// </summary>
        /// <param name="accountId">The ID of the Kweker account</param>
        /// <returns>List of products with HTTP 200 status</returns>
        [HttpGet("admin/{accountId}/products")]
        public async Task<ActionResult<IEnumerable<ProductDetails>>> GetKwekerProducts(int accountId)
        {
            try
            {
                // Verify Kweker exists using LINQ
                var kwekerExists = await _db.Kwekers.AnyAsync(k => k.Id == accountId);
                
                if (!kwekerExists)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Kweker account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Retrieve all products for this Kweker using LINQ
                var products = await _db.Products
                    .Where(p => p.KwekerId == accountId)
                    .Select(p => new ProductDetails
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        MinimumPrice = p.MinimumPrice,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        Size = p.Size,
                        KwekerId = p.KwekerId
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new { count = products.Count, products });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Kweker products: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion
    }
}
