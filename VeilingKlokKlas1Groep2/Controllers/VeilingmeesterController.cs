using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

using VeilingKlokApp.Data;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokKlas1Groep2.Models;

namespace VeilingKlokApp.Controllers
{
    [ApiController]
    [Route("api/veilingmeester")]
    public class VeilingmeesterController : ControllerBase
    {
        private readonly VeilingKlokContext _db;

        public VeilingmeesterController(VeilingKlokContext db)
        {
            _db = db;
        }

        // 1) Create Veilingmeester account (transactional)
        [HttpPost("create")]
        public async Task<IActionResult> CreateVeilingmeesterAccount([FromBody] NewVeilingMeesterAccount newVeilingmeester)
        {
            // Validate input minimally
            if (newVeilingmeester == null) return BadRequest("Missing payload.");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // NOTE: Check for existing account by email here to provide a better error message 
                // than letting the DB unique constraint fail.
                if (await _db.Accounts.AnyAsync(a => a.Email == newVeilingmeester.Email))
                {
                    await transaction.RollbackAsync();
                    return BadRequest("FAILURE: An account with this email already exists.");
                }

                // Create Veilingmeester directly (Account is abstract and base of Kweker)
                var veilingmeester = new Veilingmeester
                {
                    Email = newVeilingmeester.Email,
                    Password = newVeilingmeester.Password,
                    CreatedAt = DateTime.UtcNow,
                    Regio = newVeilingmeester.Regio,
                    SoortVeiling = newVeilingmeester.SoortVeiling
                };

                _db.Veilingmeesters.Add(veilingmeester);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "SUCCESS: Veilingmeester Account created", accountId = veilingmeester.Id });
            }
            // 👇 CATCH SPECIFIC DATABASE ERRORS
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                // This is the common exception for constraint violations, foreign key issues, etc.
                // We return the inner exception message to help you debug the exact issue.
                return StatusCode(500, $"FAILURE: Database constraint violation (rolled back). Error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"FAILURE: Account creation failed (rolled back). Error: {ex.Message}");
            }
        }

        // 2) Get Veilingmeester account
        [HttpGet("{accountId}")]
        public async Task<ActionResult<VeilingmeesterDetails>> GetVeilingmeesterAccount(int accountId)
        {
            var veilingmeesterDetails = await _db.Veilingmeesters
                .Where(v => v.Id == accountId)
                .Select(v => new VeilingmeesterDetails
                {
                    AccountId = v.Id,
                    Email = v.Email,     // base property from Account
                    Regio = v.Regio,
                    SoortVeiling = v.SoortVeiling
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (veilingmeesterDetails == null)
                return NotFound($"Velingmeester account with ID {accountId} not found.");

            return Ok(veilingmeesterDetails);
        }

        //Update Veilingmeester account (Currently does not create if it doesnt exist)
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateVeilingmeesterAccount(int accountId, [FromBody] UpdateVeilingMeester updateVeilingmeester)
        {
            // 1. Basic validation
            if (updateVeilingmeester == null)
            {
                return BadRequest("Missing payload.");
            }

            // 2. Start Transaction for atomic update of Account and Veilingmeester
            // Crucial for modifying data across two related tables (Account and Veilingmeester)
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 3. Retrieve existing Koper record
                var veilingmeester = await _db.Veilingmeesters
                    .FirstOrDefaultAsync(v => v.Id == accountId);

                // 4. Check if the resource exists
                if (veilingmeester == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound($"FAILURE: Veiliingmeester account with ID {accountId} not found. Cannot update.");
                }

                // 5. Check for Email collision if the email is being changed
                if (veilingmeester.Email != updateVeilingmeester.Email)
                {
                    // Check if the new email is already in use by a *different* account
                    if (await _db.Accounts.AnyAsync(a => a.Email == updateVeilingmeester.Email && a.Id != accountId))
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("FAILURE: Another account already uses this email address.");
                    }
                }

                // 6. Update Account details (Email)
                veilingmeester.Email = updateVeilingmeester.Email;
                // NOTE: The Password field is NOT updated here for security reasons.

                // 7. Update Koper details (Profile fields)
                veilingmeester.Password = updateVeilingmeester.Password;
                veilingmeester.SoortVeiling = updateVeilingmeester.SoortVeiling;
                veilingmeester.Regio = updateVeilingmeester.Regio;

                // EF Core tracks the changes, but we ensure they are saved
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Standard response for a successful PUT update
                return Ok(new { message = $"SUCCESS: Veilingmeester Account {accountId} updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"FAILURE: Database error during update (rolled back). Error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"FAILURE: Account update failed (rolled back). Error: {ex.Message}");
            }


        }


    }
}
