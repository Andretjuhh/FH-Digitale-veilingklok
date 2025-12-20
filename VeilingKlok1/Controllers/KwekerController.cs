using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Attributes;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Mappers;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.InputDTOs;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokApp.Utils;
using VeilingKlokKlas1Groep2.Declarations;

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
        private readonly DatabaseContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public KwekerController(
            DatabaseContext db,
            IPasswordHasher passwordHasher,
            IAuthService authService
        )
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
        public async Task<IActionResult> CreateAccount([FromBody] NewKwekerAccount newKweker)
        {
            // Validate model state (includes password strength validation)
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Check for existing account by email using LINQ
                // Provides better error message than DB unique constraint
                if (await _db.Accounts.AnyAsync(a => a.Email == newKweker.Email))
                {
                    await transaction.RollbackAsync();
                    return HtppError.Conflict("An account with this email already exists");
                }

                // Create Kweker entity using Mapper
                var kweker = KwekerMapper.ToEntity(newKweker, _passwordHasher);

                _db.Kwekers.Add(kweker);
                await _db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var authResponse = await _authService.SignInAsync(kweker, Response);
                await transaction.CommitAsync();
                return HttpSuccess<AuthResponse>.Ok(
                    authResponse,
                    "Kweker account created successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Get Account Stats

        /// <summary>
        /// Returns aggregated statistics for the authenticated Kweker
        /// </summary>
        [HttpGet("stats")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> GetKwekerStats()
        {
            try
            {
                // Get account ID from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Invalid token claims");
                }
                var accountId = accountContext.Value.AccountId;

                // Total products for this kweker
                var totalProducts = await _db
                    .Products.Where(p => p.KwekerId == accountId)
                    .CountAsync();

                // Active auctions: products that are currently assigned to a VeilingKlok (simple heuristic)
                var activeAuctions = await _db
                    .Products.Where(p => p.KwekerId == accountId && p.VeilingKlokId != null)
                    .CountAsync();

                // Stems sold and total revenue: aggregate over Orders joined with Products
                var orderQuery = _db
                    .OrderItems.Where(oi => oi.Product.KwekerId == accountId)
                    .AsQueryable();

                var stemsSold = 0;
                decimal totalRevenue = 0m;

                if (await orderQuery.AnyAsync())
                {
                    stemsSold = await orderQuery.SumAsync(o => o.Quantity);

                    // Sum revenue using the current Product.Price (approximation)
                    totalRevenue = await orderQuery
                        .Select(oi => new { oi.Quantity, ProductPrice = oi.Product.Price })
                        .SumAsync(x => x.Quantity * x.ProductPrice);
                }

                // Client Stats
                var stats = new KwekerStatsDetails
                {
                    TotalProducts = totalProducts,
                    ActiveAuctions = activeAuctions,
                    TotalRevenue = totalRevenue,
                    StemsSold = stemsSold,
                };
                return HttpSuccess<KwekerStatsDetails>.Ok(
                    stats,
                    "Kweker stats retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Get Account info

        /// <summary>
        /// Returns the authenticated Kweker's account details (current user)
        /// Requires the user to be authenticated and of account type 'Kweker'
        /// </summary>
        [HttpGet("account-info")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> GetCurrentKweker()
        {
            try
            {
                // Get account ID from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Invalid token claims");
                }

                // Retrieve Kweker using LINQ with projection to DTO
                var kwekerDetails = await _db
                    .Kwekers.Where(k => k.Id == accountContext.Value.AccountId)
                    .Select(KwekerMapper.ProjectToDetails)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (kwekerDetails == null)
                {
                    return HtppError.NotFound("Authenticated Kweker account not found");
                }

                return HttpSuccess<KwekerDetails>.Ok(
                    kwekerDetails,
                    "Kweker account retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Get products

        /// <summary>
        /// Returns all products for the authenticated Kweker (current user) with pagination
        /// Protected: Requires authentication and account type 'Kweker'
        /// </summary>
        [HttpGet("products")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> GetProducts([FromQuery] int page = 1)
        {
            try
            {
                // Get account ID from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Invalid token claims");
                }

                // Get pagination info
                var pagination = PaginationHelper.GetPagination(page);

                // Get total count of products for this Kweker
                var totalCount = await _db
                    .Products.Where(p => p.KwekerId == accountContext.Value.AccountId)
                    .CountAsync();

                // Retrieve paginated products for this Kweker
                var products = await _db
                    .Products.Where(p => p.KwekerId == accountContext.Value.AccountId)
                    .OrderByDescending(p => p.Id)
                    .Skip(pagination.Offset)
                    .Take(pagination.Limit)
                    .Select(ProductMapper.ProjectToDetails)
                    .AsNoTracking()
                    .ToListAsync();

                // Create paginated response
                var response = new PaginatedResponse<ProductDetails>
                {
                    Page = pagination.Page,
                    Limit = pagination.Limit,
                    TotalCount = totalCount,
                    Data = products,
                };

                return HttpSuccess<PaginatedResponse<ProductDetails>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion
        #region Product Management

        /// <summary>
        /// Creates a new product for a kweker (grower)
        /// </summary>
        /// <param name="newProduct">Product details to create</param>
        /// <returns>Created product details with HTTP 201 status</returns>
        [HttpPost("product/create")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> CreateProduct([FromBody] NewProduct newProduct)
        {
            // Validate model state (includes required field validation)
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Get authenticated account id (set by AuthorizeAttribute)
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    await transaction.RollbackAsync();
                    return HtppError.Unauthorized("Authenticated account not found in request");
                }

                // Verify the authenticated account exists as a Kweker
                var kwekerExists = await _db.Kwekers.AnyAsync(k =>
                    k.Id == accountContext.Value.AccountId
                );

                // If not a valid Kweker, return Forbidden
                if (!kwekerExists)
                {
                    await transaction.RollbackAsync();
                    return HtppError.Forbidden("Authenticated account is not a valid Kweker");
                }

                // Create the product entity using Mapper
                var product = ProductMapper.ToEntity(newProduct, accountContext.Value.AccountId);

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO
                var productDetails = ProductMapper.ToDetails(product);

                return HttpSuccess<ProductDetails>.Created(
                    productDetails,
                    "Product created successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="productId">The ID of the product to update</param>
        /// <param name="updateProduct">Updated product details</param>
        /// <returns>Updated product details with HTTP 200 status</returns>
        [HttpPut("product/update/{productId}")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> UpdateProduct(
            Guid productId,
            [FromBody] UpdateProduct updateProduct
        )
        {
            // Validate model state
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the product using LINQ
                var product = await _db
                    .Products.Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound($"Product with ID {productId} not found");
                }

                // Update entity using Mapper
                ProductMapper.UpdateEntity(product, updateProduct);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return updated product details
                return HttpSuccess<ProductDetails>.Ok(
                    ProductMapper.ToDetails(product),
                    "Product updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return HtppError
                    .InternalServerError(
                        $"Database constraint violation: {ex.InnerException?.Message ?? ex.Message}"
                    )
                    .WithDetails("Database Error");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion
    }
}
