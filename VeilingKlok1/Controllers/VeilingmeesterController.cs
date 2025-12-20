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
using VeilingKlokApp.Utils;
using VeilingKlokKlas1Groep2.Declarations;

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
        private readonly DatabaseContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthService _authService;

        public VeilingmeesterController(
            DatabaseContext db,
            IPasswordHasher passwordHasher,
            IAuthService authService
        )
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
        public async Task<IActionResult> CreateAccount(
            [FromBody] NewVeilingMeesterAccount newVeilingmeester
        )
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
                if (await _db.Accounts.AnyAsync(a => a.Email == newVeilingmeester.Email))
                {
                    await transaction.RollbackAsync();
                    return HtppError.Conflict("An account with this email already exists");
                }

                // Create Veilingmeester entity using Mapper
                var veilingmeester = VeilingmeesterMapper.ToEntity(
                    newVeilingmeester,
                    _passwordHasher
                );

                _db.Veilingmeesters.Add(veilingmeester);
                await _db.SaveChangesAsync();

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var authResponse = await _authService.SignInAsync(veilingmeester, Response);

                await transaction.CommitAsync();

                return HttpSuccess<AuthResponse>.Ok(
                    authResponse,
                    "Veilingmeester account created successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
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
        public async Task<IActionResult> GetVeilingmeesterAccount(Guid accountId)
        {
            try
            {
                // Retrieve Veilingmeester using LINQ with projection to DTO
                var veilingmeesterDetails = await _db
                    .Veilingmeesters.Where(v => v.Id == accountId)
                    .Select(VeilingmeesterMapper.ProjectToDetails)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (veilingmeesterDetails == null)
                {
                    return HtppError.NotFound(
                        $"Veilingmeester account with ID {accountId} not found"
                    );
                }

                return HttpSuccess<VeilingmeesterDetails>.Ok(
                    veilingmeesterDetails,
                    "Veilingmeester account retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
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
        [HttpPut("update")]
        public async Task<IActionResult> UpdateVeilingmeesterAccount(
            Guid accountId,
            [FromBody] UpdateVeilingMeester updateVeilingmeester
        )
        {
            // Validate model state
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            // Start transaction for atomic update across Account and Veilingmeester tables
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve existing Veilingmeester record using LINQ
                var veilingmeester = await _db
                    .Veilingmeesters.Where(v => v.Id == accountId)
                    .FirstOrDefaultAsync();

                // Check if the resource exists
                if (veilingmeester == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound(
                        $"Veilingmeester account with ID {accountId} not found"
                    );
                }

                // Check for email collision if email is being changed
                if (veilingmeester.Email != updateVeilingmeester.Email)
                {
                    // Verify new email isn't used by a different account using LINQ
                    var emailExists = await _db.Accounts.AnyAsync(a =>
                        a.Email == updateVeilingmeester.Email && a.Id != accountId
                    );

                    if (emailExists)
                    {
                        await transaction.RollbackAsync();
                        return HtppError.Conflict(
                            "Another account already uses this email address"
                        );
                    }
                }

                // Update entity using Mapper
                VeilingmeesterMapper.UpdateEntity(veilingmeester, updateVeilingmeester);

                // Save changes and commit transaction
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return HttpSuccess<object>.Ok(
                    new { message = $"Veilingmeester account {accountId} updated successfully" },
                    $"Veilingmeester account {accountId} updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return HtppError
                    .InternalServerError(
                        $"Database error during update: {ex.InnerException?.Message ?? ex.Message}"
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

        #region Get Veilingmeester Auction Clocks

        /// <summary>
        /// Retrieves all VeilingKlokken (Auction Clocks) for a specific Veilingmeester
        /// </summary>
        /// <param name="accountId">The ID of the Veilingmeester account</param>
        /// <returns>List of VeilingKlokken with HTTP 200 status</returns>
        [HttpGet("veilingklokken")]
        public async Task<IActionResult> GetVeilingmeesterVeilingKlokken(Guid accountId)
        {
            try
            {
                // Verify Veilingmeester exists using LINQ
                var veilingmeesterExists = await _db.Veilingmeesters.AnyAsync(v =>
                    v.Id == accountId
                );

                if (!veilingmeesterExists)
                {
                    return HtppError.NotFound(
                        $"Veilingmeester account with ID {accountId} not found"
                    );
                }

                // Retrieve all VeilingKlokken for this Veilingmeester using LINQ
                var veilingKlokken = await _db
                    .Veilingklokken.Where(vk => vk.VeilingmeesterId == accountId)
                    .Select(VeilingKlokMapper.ProjectToDetails)
                    .OrderBy(vk => vk.StartTime)
                    .AsNoTracking()
                    .ToListAsync();

                return HttpSuccess<object>.Ok(
                    new { count = veilingKlokken.Count, veilingKlokken },
                    "VeilingKlokken retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion
    }
}
