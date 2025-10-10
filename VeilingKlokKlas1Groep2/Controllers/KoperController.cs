
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace WebProject_Klas1_Groep2.Data;
using WebProject_Klas1_Groep2.Models;
    
[ApiController]
[Route("[controller]")]
public class KoperController : ControllerBase
{
    private readonly VeilingKlokContext _db;

    public KoperController(VeilingKlokContext db)
    {
        _db = db;
    }

    // 1) Create Koper account (transactional)
    [HttpPost("koper/create")]
    public async Task<IActionResult> CreateKoperAccount([FromBody] NewKoperAccount newKoper)
    {
        // Validate input minimally
        if (newKoper == null) return BadRequest("Missing payload.");

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Create Account
            var account = new Account
            {
                Email = newKoper.Email,
                Password = newKoper.Password
            };

            // NOTE: Check for existing account by email here to provide a better error message 
            // than letting the DB unique constraint fail.
            if (await _db.Accounts.AnyAsync(a => a.Email == newKoper.Email))
            {
                await transaction.RollbackAsync();
                return BadRequest("FAILURE: An account with this email already exists.");
            }

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(); // account.Id gets populated

            // Create Koper; AccountId is PK/FK for Koper
            var koper = new Koper
            {
                AccountId = account.Id,
                FirstName = newKoper.FirstName,
                LastName = newKoper.LastName,
                Adress = newKoper.Adress,
                PostCode = newKoper.PostCode,
                Regio = newKoper.Regio
            };

            _db.Kopers.Add(koper);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new { message = "SUCCESS: Koper Account created", accountId = account.Id });
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

    // 2) Get Koper account
    [HttpGet("koper/{accountId}")]
    public async Task<ActionResult<KoperDetails>> GetKoperAccount(int accountId)
    {
        var koperDetails = await _db.Kopers
            .Where(k => k.AccountId == accountId)
            .Select(k => new KoperDetails
            {
                AccountId = k.AccountId,
                Email = k.Account.Email,     // uses navigation to Account
                FirstName = k.FirstName,
                LastName = k.LastName,
                Adress = k.Adress,
                Regio = k.Regio
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (koperDetails == null)
            return NotFound($"Koper account with ID {accountId} not found.");

        return Ok(koperDetails);
    }

    
}