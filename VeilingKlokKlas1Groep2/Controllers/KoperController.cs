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
using VeilingKlokKlas1Groep2.Services;
using VeilingKlokKlas1Groep2.Configuration;

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
        private readonly VeilingKlokContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public KoperController(VeilingKlokContext db, IPasswordHasher passwordHasher, IAuthService authService)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        #region Create Koper Account

        /// <summary>
        /// Creates a new Koper (Buyer) account
        /// </summary>
        /// <param name="newKoper">Koper account details to create</param>
        /// <returns>Created account ID with HTTP 200 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateKoperAccount([FromBody] NewKoperAccount newKoper)
        {
            // Validate input payload
            if (newKoper == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Koper account data is required",
                    400
                );
                return BadRequest(error);
            }

            // Validate model state (includes password strength validation)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var error = new HtppError(
                    "Validation Error",
                    string.Join(". ", errors),
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Check for existing account by email using LINQ
                // Provides better error message than DB unique constraint
                if (await _db.Accounts.AnyAsync(a => a.Email == newKoper.Email))
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Conflict",
                        "An account with this email already exists",
                        409
                    );
                    return Conflict(error);
                }

                // Create Koper directly (Account is abstract and base of Koper)
                var koper = new Koper
                {
                    Email = newKoper.Email,
                    Password = _passwordHasher.HashPassword(newKoper.Password), // Hash the password
                    CreatedAt = DateTime.UtcNow,
                    FirstName = newKoper.FirstName,
                    LastName = newKoper.LastName,
                    Adress = newKoper.Adress,
                    PostCode = newKoper.PostCode,
                    Regio = newKoper.Regio
                };

                _db.Kopers.Add(koper);
                await _db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var accountType = "Koper";
                var authResponse = await _authService.SignInAsync(koper, accountType, Response);

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

        #region Get Koper Account

        /// <summary>
        /// Retrieves a Koper account by ID
        /// Protected: Users can only access their own account information
        /// </summary>
        /// <param name="accountId">The ID of the Koper account to retrieve</param>
        /// <returns>Koper account details with HTTP 200 status</returns>
        [HttpGet("{accountId}")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeOwner("accountId")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Koper")]
        public async Task<ActionResult<KoperDetails>> GetKoperAccount(int accountId)
        {
            try
            {
                // Retrieve Koper using LINQ with projection to DTO
                var koperDetails = await _db.Kopers
                    .Where(k => k.Id == accountId)
                    .Select(k => new KoperDetails
                    {
                        AccountId = k.Id,
                        Email = k.Email,     // Base property from Account
                        FirstName = k.FirstName,
                        LastName = k.LastName,
                        Adress = k.Adress,
                        Regio = k.Regio
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (koperDetails == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Koper account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(koperDetails);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Koper account: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Update Koper Account

        /// <summary>
        /// Updates an existing Koper account
        /// Protected: Users can only update their own account
        /// </summary>
        /// <param name="accountId">The ID of the Koper account to update</param>
        /// <param name="updateKoper">Updated Koper profile details</param>
        /// <returns>Success message with HTTP 200 status</returns>
        [HttpPut("update/{accountId}")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeOwner("accountId")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Koper")]
        public async Task<IActionResult> UpdateKoperAccount(int accountId, [FromBody] UpdateKoperProfile updateKoper)
        {
            // Validate input payload
            if (updateKoper == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Update data is required",
                    400
                );
                return BadRequest(error);
            }

            // Start transaction for atomic update across Account and Koper tables
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve existing Koper record using LINQ
                var koper = await _db.Kopers
                    .Where(k => k.Id == accountId)
                    .FirstOrDefaultAsync();

                // Check if the resource exists
                if (koper == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Koper account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Check for email collision if email is being changed
                if (koper.Email != updateKoper.Email)
                {
                    // Verify new email isn't used by a different account using LINQ
                    var emailExists = await _db.Accounts
                        .AnyAsync(a => a.Email == updateKoper.Email && a.Id != accountId);

                    if (emailExists)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Conflict",
                            "Another account already uses this email address",
                            409
                        );
                        return Conflict(error);
                    }
                }

                // Update Account details (Email)
                koper.Email = updateKoper.Email;

                // Update Koper details (Profile fields)
                // Hash password if it's being updated
                if (!string.IsNullOrWhiteSpace(updateKoper.Password))
                {
                    koper.Password = _passwordHasher.HashPassword(updateKoper.Password);
                }
                koper.FirstName = updateKoper.FirstName;
                koper.LastName = updateKoper.LastName;
                koper.Adress = updateKoper.Adress;
                koper.PostCode = updateKoper.PostCode;
                koper.Regio = updateKoper.Regio;

                // Save changes and commit transaction
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"Koper account {accountId} updated successfully" });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Database Error",
                    $"Database error during update: {ex.InnerException?.Message ?? ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Internal Server Error",
                    $"Account update failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get Koper Orders

        /// <summary>
        /// Retrieves a Koper account with all associated orders
        /// Protected: Users can only access their own orders
        /// </summary>
        /// <param name="accountId">The ID of the Koper account</param>
        /// <returns>Koper details with orders with HTTP 200 status</returns>
        [HttpGet("{accountId}/orders")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeOwner("accountId")]
        [VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Koper")]
        public async Task<ActionResult<KoperDetailsWithOrders>> GetKoperOrders(int accountId)
        {
            try
            {
                // Retrieve Koper with orders using LINQ projection
                var koperDetails = await _db.Kopers
                    .Where(k => k.Id == accountId)
                    .Select(k => new KoperDetailsWithOrders
                    {
                        AccountId = k.Id,
                        Email = k.Email,     // Base property from Account
                        FirstName = k.FirstName,
                        LastName = k.LastName,
                        Adress = k.Adress,
                        Regio = k.Regio,
                        // Project the related Orders collection using LINQ
                        Orders = k.Orders.Select(o => new OrderDetails
                        {
                            Id = o.Id,
                            Quantity = o.Quantity,
                            BoughtAt = o.BoughtAt
                        }).ToList()
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (koperDetails == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Koper account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(koperDetails);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Koper orders: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        // Helpers moved to AuthService
    }
}