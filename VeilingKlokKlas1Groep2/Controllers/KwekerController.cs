
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace VeilingKlokKlas1Groep2.Data;
using VeilingKlokKlas1Groep2.Models.Domain;
using VeilingKlokKlas1Groep2.Models;
    
[ApiController]
[Route("[controller]")]
public class KwekerController : ControllerBase
{
    private readonly VeilingKlokContext _db;

    public KwekerController(VeilingKlokContext db)
    {
        _db = db;
    }

    // 1) Create Kweker account (transactional)
    [HttpPost("kweker/create")]
    public async Task<IActionResult> CreateKwekerAccount([FromBody] NewKwekerAccount newKweker)
    {
        // Validate input minimally
        if (newKweker == null) return BadRequest("Missing payload.");

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Create Account
            var account = new Account
            {
                Email = newKweker.Email,
                Password = newKweker.Password,
                CreatedAt = DateTime.UtcNow
            };

            // NOTE: Check for existing account by email here to provide a better error message 
            // than letting the DB unique constraint fail.
            if (await _db.Accounts.AnyAsync(a => a.Email == newKweker.Email))
            {
                await transaction.RollbackAsync();
                return BadRequest("FAILURE: An account with this email already exists.");
            }

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(); // account.Id gets populated

            // Create Kweker; AccountId is PK/FK for Kweker
            var kweker = new Kweker
            {
                AccountId = account.Id,
                Name = newKweker.Name,
                Telephone = newKweker.Telephone,
                Adress = newKweker.Adress,
                Regio = newKweker.Regio,
                KvkNumber = newKweker.KvkNumber
            };

            _db.Kwekers.Add(kweker);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new { message = "SUCCESS: Kweker Account created", accountId = account.Id });
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

    // 2) Get Kweker account
    [HttpGet("kweker/{accountId}")]
    public async Task<ActionResult<KwekerDetails>> GetKwekerAccount(int accountId)
    {
        var kwekerDetails = await _db.Kwekers
            .Where(k => k.AccountId == accountId)
            .Select(k => new KwekerDetails
            {
                AccountId = k.AccountId,
                Name = k.Name,
                Email = k.Account.Email,     // uses navigation to Account
                Telephone = k.Telephone,
                Adress = k.Adress,
                Regio = k.Regio,
                KvkNumber = k.KvkNumber,
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (kwekerDetails == null)
            return NotFound($"Koper account with ID {accountId} not found.");

        return Ok(kwekerDetails);
    }

    
}
