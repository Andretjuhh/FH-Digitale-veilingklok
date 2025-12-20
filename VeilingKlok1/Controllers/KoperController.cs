using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Attributes;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Mappers;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokApp.Utils;
using VeilingKlokKlas1Groep2.Declarations;
using static VeilingKlokApp.Utils.PaginationHelper;

namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for managing Koper (Buyer) operations
    /// Handles CRUD operations for buyer accounts and their orders
    /// </summary>
    [ApiController]
    [Route("api/koper")]
    public class KoperController : ControllerBase
    {
        private readonly DatabaseContext db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public KoperController(
            DatabaseContext db,
            IPasswordHasher passwordHasher,
            IAuthService authService
        )
        {
            this.db = db;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        #region Create Koper Account

        /// <summary>
        /// Creates a new Koper (Buyer) account
        /// </summary>
        /// <param name="newKoper">Koper account details to create</param>
        /// <returns>Authentication response with JWT tokens and HTTP 201 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] NewKoperAccount newKoper)
        {
            // Validate model state (includes password strength validation)
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            // Start transaction for atomic operation across Account and Koper tables
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // Check for existing account by email using LINQ
                // Provides better error message than DB unique constraint
                if (await db.Accounts.AnyAsync(a => a.Email == newKoper.Email))
                {
                    await transaction.RollbackAsync();
                    return HtppError.Conflict("An account with this email already exists");
                }

                // Create Koper directly using Mapper
                var koper = KoperMapper.ToEntity(newKoper, _passwordHasher);

                db.Kopers.Add(koper);
                await db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var authResponse = await _authService.SignInAsync(koper, Response);
                await transaction.CommitAsync();
                return HttpSuccess<AuthResponse>.Created(
                    authResponse,
                    "Koper account created successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Get Koper Account

        /// <summary>
        /// Retrieves a Koper account by ID
        /// Protected: Users can only access their own account information
        /// </summary>
        /// <returns>Koper account details with HTTP 200 status</returns>
        [HttpGet("account")]
        [AuthorizeAccountType(AccountType.Koper)]
        public async Task<IActionResult> GetAccount(Guid accountId)
        {
            try
            {
                // Get account ID and account type from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);

                // Check if account ID and account type are valid
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Account is not authenticated");
                }

                // Retrieve Koper using LINQ with projection to DTO
                var koperDetails = await db
                    .Kopers.Where(k => k.Id == accountContext.Value.AccountId)
                    .Select(KoperMapper.ProjectToDetails)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (koperDetails == null)
                {
                    return HtppError.NotFound($"Koper account with ID {accountId} not found");
                }

                return HttpSuccess<KoperDetails>.Ok(koperDetails);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Update Koper Account

        /// <summary>
        /// Updates an existing Koper account
        /// Protected: Users can only update their own account
        /// </summary>
        /// <param name="updateKoper">Updated Koper profile details</param>
        /// <returns>Success message with HTTP 204 status</returns>
        [HttpPut("update")]
        [AuthorizeAccountType(AccountType.Koper)]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateKoperProfile updateKoper)
        {
            // Validate model state (includes password strength validation if password provided)
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            // Start transaction for atomic update across Account and Koper tables
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                // Get account ID and account type from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);

                // Check if account ID and account type are valid
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Account is not authenticated");
                }

                // Retrieve existing Koper record using LINQ
                var koper = await db
                    .Kopers.Where(k => k.Id == accountContext.Value.AccountId)
                    .FirstOrDefaultAsync();

                // Check if the resource exists
                if (koper == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound(
                        $"Koper account with ID {accountContext.Value.AccountId} not found"
                    );
                }

                // Check for email collision if email is being changed
                if (
                    !string.IsNullOrWhiteSpace(updateKoper.Email)
                    && koper.Email != updateKoper.Email
                )
                {
                    // Verify new email isn't used by a different account using LINQ
                    var emailExists = await db.Accounts.AnyAsync(a =>
                        a.Email == updateKoper.Email && a.Id != accountContext.Value.AccountId
                    );

                    if (emailExists)
                    {
                        await transaction.RollbackAsync();
                        return HtppError.Conflict(
                            "Another account already uses this email address"
                        );
                    }
                }

                // Use Mapper to update entity fields (including password hashing)
                KoperMapper.UpdateEntity(koper, updateKoper, _passwordHasher);

                // Save changes and commit transaction
                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return success response
                return HttpSuccess<object>.NoContent(
                    $"Koper account {accountContext.Value.AccountId} updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Get Koper Orders

        /// <summary>
        /// Retrieves paginated orders for the authenticated Koper account
        /// with product details, sorted by most recent orders first.
        /// Protected: Users can only access their own orders
        /// </summary>
        /// <param name="page">Page number for pagination (1-based, default: 1)</param>
        /// <returns>Paginated response with orders and product details, HTTP 200 status</returns>
        [HttpGet("orders")]
        [AuthorizeAccountType(AccountType.Koper)]
        public async Task<IActionResult> GetAccountOrders([FromQuery] int page = 1)
        {
            try
            {
                // Get account ID and account type from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);

                // Check if account ID and account type are valid
                if (!accountContext.HasValue)
                    return HtppError.Unauthorized("Account is not authenticated");

                // Get pagination info
                var pagination = GetPagination(page);

                // Get total count of orders for this Koper
                var totalCount = await db
                    .Orders.Where(o => o.KoperId == accountContext.Value.AccountId)
                    .CountAsync();

                // Retrieve paginated orders with product details
                var orders = await db
                    .Orders.Where(o => o.KoperId == accountContext.Value.AccountId)
                    .OrderByDescending(o => o.BoughtAt) // Newest to oldest
                    .Skip(pagination.Offset)
                    .Take(pagination.Limit)
                    .Select(o => new OrderWithItemsDetails
                    {
                        OrderId = o.Id,
                        CreatedAt = o.BoughtAt,
                        Products = o
                            .OrderItems.Select(oi => new OrderItemDetails
                            {
                                ProductId = oi.Product.Id,
                                ProductName = oi.Product.Name,
                                Price = oi.PriceAtPurchase,
                                QuantityBought = oi.Quantity,
                                ImageUrl = oi.Product.ImageUrl,
                            })
                            .ToList(),
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // Create paginated response
                var response = new PaginatedResponse<OrderWithItemsDetails>
                {
                    Page = pagination.Page,
                    Limit = pagination.Limit,
                    TotalCount = totalCount,
                    Data = orders,
                };

                return HttpSuccess<PaginatedResponse<OrderWithItemsDetails>>.Ok(response);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion
    }
}
