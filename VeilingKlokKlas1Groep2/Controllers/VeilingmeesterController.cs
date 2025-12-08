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
using VeilingKlokKlas1Groep2.Models;
using VeilingKlokKlas1Groep2.Services;
 
namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for managing Veilingmeester (Auctioneer) operations
    /// Handles CRUD operations for auctioneer accounts and their auction clocks
    /// </summary>
    [ApiController]
    [Route("api/veilingmeester")]
    public class VeilingmeesterController : ControllerBase
    {
        private readonly VeilingKlokContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public VeilingmeesterController(VeilingKlokContext db, IPasswordHasher passwordHasher, IAuthService authService)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        #region Create Veilingmeester Account

        /// <summary>
        /// Creates a new Veilingmeester (Auctioneer) account
        /// </summary>
        /// <param name="newVeilingmeester">Veilingmeester account details to create</param>
        /// <returns>Created account ID with HTTP 200 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateVeilingmeesterAccount([FromBody] NewVeilingMeesterAccount newVeilingmeester)
        {
            // Validate input payload
            if (newVeilingmeester == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Veilingmeester account data is required",
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Check for existing account by email using LINQ
                // Provides better error message than DB unique constraint
                if (await _db.Accounts.AnyAsync(a => a.Email == newVeilingmeester.Email))
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Conflict",
                        "An account with this email already exists",
                        409
                    );
                    return Conflict(error);
                }

                // Create Veilingmeester entity (Account is abstract base class)
                var veilingmeester = new Veilingmeester
                {
                    Email = newVeilingmeester.Email,
                    Password = _passwordHasher.HashPassword(newVeilingmeester.Password),
                    CreatedAt = DateTime.UtcNow,
                    Regio = newVeilingmeester.Regio,
                    AuthorisatieCode = newVeilingmeester.AuthorisatieCode
                };

                _db.Veilingmeesters.Add(veilingmeester);
                await _db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var accountType = "Veilingmeester";
                var authResponse = await _authService.SignInAsync(veilingmeester, accountType, Response);

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

        #region Get Veilingmeester Account

        /// <summary>
        /// Retrieves a Veilingmeester account by ID
        /// </summary>
        /// <param name="accountId">The ID of the Veilingmeester account to retrieve</param>
        /// <returns>Veilingmeester account details with HTTP 200 status</returns>
        [HttpGet("{accountId}")]
        public async Task<ActionResult<VeilingmeesterDetails>> GetVeilingmeesterAccount(int accountId)
        {
            try
            {
                // Retrieve Veilingmeester using LINQ with projection to DTO
                var veilingmeesterDetails = await _db.Veilingmeesters
                    .Where(v => v.Id == accountId)
                    .Select(v => new VeilingmeesterDetails
                    {
                        AccountId = v.Id,
                        Email = v.Email,     // Base property from Account
                        Regio = v.Regio,
                        AuthorisatieCode = v.AuthorisatieCode
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (veilingmeesterDetails == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Veilingmeester account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(veilingmeesterDetails);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Veilingmeester account: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Update Veilingmeester Account

        /// <summary>
        /// Updates an existing Veilingmeester account
        /// </summary>
        /// <param name="accountId">The ID of the Veilingmeester account to update</param>
        /// <param name="updateVeilingmeester">Updated Veilingmeester profile details</param>
        /// <returns>Success message with HTTP 200 status</returns>
        [HttpPut("update/{accountId}")]
        public async Task<IActionResult> UpdateVeilingmeesterAccount(int accountId, [FromBody] UpdateVeilingMeester updateVeilingmeester)
        {
            // Validate input payload
            if (updateVeilingmeester == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Update data is required",
                    400
                );
                return BadRequest(error);
            }

            // Start transaction for atomic update across Account and Veilingmeester tables
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve existing Veilingmeester record using LINQ
                var veilingmeester = await _db.Veilingmeesters
                    .Where(v => v.Id == accountId)
                    .FirstOrDefaultAsync();

                // Check if the resource exists
                if (veilingmeester == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Veilingmeester account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Check for email collision if email is being changed
                if (veilingmeester.Email != updateVeilingmeester.Email)
                {
                    // Verify new email isn't used by a different account using LINQ
                    var emailExists = await _db.Accounts
                        .AnyAsync(a => a.Email == updateVeilingmeester.Email && a.Id != accountId);

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
                veilingmeester.Email = updateVeilingmeester.Email;

                // Update Veilingmeester details (Profile fields)
                veilingmeester.Password = updateVeilingmeester.Password;
                veilingmeester.AuthorisatieCode = updateVeilingmeester.AuthorisatieCode;
                veilingmeester.Regio = updateVeilingmeester.Regio;

                // Save changes and commit transaction
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"Veilingmeester account {accountId} updated successfully" });
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

        #region Get Veilingmeester Auction Clocks

        /// <summary>
        /// Retrieves all VeilingKlokken (Auction Clocks) for a specific Veilingmeester
        /// </summary>
        /// <param name="accountId">The ID of the Veilingmeester account</param>
        /// <returns>List of VeilingKlokken with HTTP 200 status</returns>
        [HttpGet("{accountId}/veilingklokken")]
        public async Task<ActionResult<IEnumerable<VeilingKlokDetails>>> GetVeilingmeesterVeilingKlokken(int accountId)
        {
            try
            {
                // Verify Veilingmeester exists using LINQ
                var veilingmeesterExists = await _db.Veilingmeesters.AnyAsync(v => v.Id == accountId);
                
                if (!veilingmeesterExists)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Veilingmeester account with ID {accountId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Retrieve all VeilingKlokken for this Veilingmeester using LINQ
                var veilingKlokken = await _db.Veilingklokken
                    .Where(vk => vk.VeilingmeesterId == accountId)
                    .Select(vk => new VeilingKlokDetails
                    {
                        Id = vk.Id,
                        Naam = vk.Naam,
                        DurationInSeconds = vk.DurationInSeconds,
                        LiveViews = vk.LiveViews,
                        StartTime = vk.StartTime,
                        EndTime = vk.EndTime,
                        VeilingmeesterId = vk.VeilingmeesterId
                    })
                    .OrderBy(vk => vk.StartTime)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new { count = veilingKlokken.Count, veilingKlokken });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve Veilingmeester auction clocks: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

     }
}
