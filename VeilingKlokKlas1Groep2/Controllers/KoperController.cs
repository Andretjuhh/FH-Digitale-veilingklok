using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

using VeilingKlokApp.Data;
using VeilingKlokApp.Models; 
using VeilingKlokApp.Models.Domain;

namespace VeilingKlokApp.Controllers
{

    [ApiController]
    [Route("api/koper")] // Is a convetion to route is everything before "Controller"
    public class KoperController : ControllerBase
    {
        private readonly VeilingKlokContext _db;

        public KoperController(VeilingKlokContext db)
        {
            _db = db;
        }

        // 1) Create Koper account (transactional)
        [HttpPost("create")]
        public async Task<IActionResult> CreateKoperAccount([FromBody] NewKoperAccount newKoper)
        {
            // Validate input minimally
            if (newKoper == null) return BadRequest("Missing payload.");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // NOTE: Check for existing account by email here to provide a better error message 
                // than letting the DB unique constraint fail.
                if (await _db.Accounts.AnyAsync(a => a.Email == newKoper.Email))
                {
                    await transaction.RollbackAsync();
                    return BadRequest("FAILURE: An account with this email already exists.");
                }

                // Create Koper directly (Account is abstract and base of Koper)
                var koper = new Koper
                {
                    Email = newKoper.Email,
                    Password = newKoper.Password,
                    FirstName = newKoper.FirstName,
                    LastName = newKoper.LastName,
                    Adress = newKoper.Adress,
                    PostCode = newKoper.PostCode,
                    Regio = newKoper.Regio
                };

                _db.Kopers.Add(koper);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return the standard 201 Created response with a success message and the new account ID
                 return Ok(new { message= "SUCCESS: Kweker Account Created.", accountId = koper.Id });)
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
        [HttpGet("{accountId}")]
        public async Task<ActionResult<KoperDetails>> GetKoperAccount(int accountId)
        {
            var koperDetails = await _db.Kopers
                .Where(k => k.Id == accountId)
                .Select(k => new KoperDetails
                {
                    AccountId = k.Id,
                    Email = k.Email,     // base property from Account
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

        //Update Koper account (Currently does not create if it doesnt exist)
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateKoperAccount(int accountId, [FromBody] UpdateKoperProfile updateKoper)
        {
            // 1. Basic validation
            if (updateKoper == null)
            {
                return BadRequest("Missing payload.");
            }

            // 2. Start Transaction for atomic update of Account and Koper
            // Crucial for modifying data across two related tables (Account and Koper)
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 3. Retrieve existing Koper record
                var koper = await _db.Kopers
                    .FirstOrDefaultAsync(k => k.Id == accountId);

                // 4. Check if the resource exists
                if (koper == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound($"FAILURE: Koper account with ID {accountId} not found. Cannot update.");
                }

                // 5. Check for Email collision if the email is being changed
                if (koper.Email != updateKoper.Email)
                {
                    // Check if the new email is already in use by a *different* account
                    if (await _db.Accounts.AnyAsync(a => a.Email == updateKoper.Email && a.Id != accountId))
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("FAILURE: Another account already uses this email address.");
                    }
                }

                // 6. Update Account details (Email)
                koper.Email = updateKoper.Email;
                // NOTE: The Password field is NOT updated here for security reasons.

                // 7. Update Koper details (Profile fields)
                koper.FirstName = updateKoper.FirstName;
                koper.LastName = updateKoper.LastName;
                koper.Adress = updateKoper.Adress;
                koper.PostCode = updateKoper.PostCode;
                koper.Regio = updateKoper.Regio;

                // EF Core tracks the changes, but we ensure they are saved
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Standard response for a successful PUT update
                return Ok(new { message = $"SUCCESS: Koper Account {accountId} updated successfully." });
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
       
        // 4) Get Orders of koper
        [HttpGet("{accountId}/order")]
        public async Task<ActionResult<KoperDetailsWithOrders>> GetKoperOrders(int accountId)
        {
            var koperDetails = await _db.Kopers
                .Where(k => k.Id == accountId)
                .Select(k => new KoperDetailsWithOrders
                {
                    AccountId = k.Id,
                    Email = k.Email,     // base property from Account
                    FirstName = k.FirstName,
                    LastName = k.LastName,
                    Adress = k.Adress,
                    Regio = k.Regio,
                    // CHANGE 3: Project the related Orders collection
                    Orders = k.Orders.Select(o => new OrderDetails // Use a lightweight Order DTO
                    {
                        Id = o.Id,
                        Quantity = o.Quantity,
                        BoughtAt = o.BoughtAt
                        // Include other necessary order fields here
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (koperDetails == null)
                return NotFound($"Koper account with ID {accountId} not found.");

            return Ok(koperDetails);
        }

    }

}