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

    }
}
