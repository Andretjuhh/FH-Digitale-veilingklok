
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace WebProject_Klas1_Groep2.Data;
using WebProject_Klas1_Groep2.Models;

[ApiController]
[Route("[controller]")]
public class VeilingKlokController : ControllerBase
{
    private readonly VeilingKlokContext _db;

    public VeilingKlokController(VeilingKlokContext db)
    {
        _db = db;
    }

    // 1) Connection test
    [HttpGet("test")]
    public IActionResult TestConnection()
    {
        try
        {
            if (_db.Database.CanConnect())
                return Ok("SUCCESS: Connected to VeilingKlok database via VeilingKlokDB catalog!");
            else
                return StatusCode(500, "FAILURE: Cannot connect to database.");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"FAILURE: Connection failed. Error: {ex.Message}");
        }
    }

    // 2) Create Koper account (transactional)
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
                Adress = newKoper.Address,
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

    // 3) Get Koper account
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
                Address = k.Adress,
                Regio = k.Regio
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (koperDetails == null)
            return NotFound($"Koper account with ID {accountId} not found.");

        return Ok(koperDetails);
    }
}





// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Data.SqlClient;
// using System.Collections.Generic;
// using System.Data;
// using System;

// namespace WebProject_Klas1_Groep2.Controllers
// {
//     // Sets the URL route for this controller (e.g., /VeilingKlok)
//     [ApiController]
//     [Route("[controller]")]

//     public class VeilingKlokController : ControllerBase
//     {
//         // Define the connection string. Using 'VeilingKlokDB' because your tables landed there.
//         const string connectionString =
//             "Data Source=(localdb)\\MSSQLLocalDB;" +
//             "Initial Catalog=VeilingKlokDB;";

//         // --------------------------------------------------------------------------------
//         // 1. Connection Test (Existing method)
//         // --------------------------------------------------------------------------------

//         [HttpGet]
//         [Route("test")]
//         public string TestConnection()
//         {
//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 try
//                 {
//                     connection.Open();
//                     return "SUCCESS: Connected to VeilingKlok database via VeilingKlokDB catalog!";
//                 }
//                 catch (Exception ex)
//                 {
//                     return $"FAILURE: Connection failed. Error: {ex.Message}";
//                 }
//             }
//         }

//         // --------------------------------------------------------------------------------
//         // 2. Create Koper Account (New method using Transactions)
//         // --------------------------------------------------------------------------------

//         [HttpPost]
//         [Route("koper/create")]
//         public string CreateKoperAccount([FromBody] NewKoperAccount newKoper)
//         {
//             // SQL for the first INSERT (Account) and to get the new ID
//             const string insertAccountSql =
//                 "INSERT INTO Account (email, password) VALUES (@email, @password); " +
//                 "SELECT SCOPE_IDENTITY();";

//             // SQL for the second INSERT (Koper)
//             const string insertKoperSql =
//                 "INSERT INTO Koper (account_id, first_name, last_name, adress, post_code, regio) " +
//                 "VALUES (@accountId, @firstName, @lastName, @address, @postCode, @regio);";

//             Int32 newAccountId = 0;

//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 connection.Open();
//                 SqlTransaction sqlTran = connection.BeginTransaction(); // BEGIN TRANSACTION

//                 // Command objects linked to the transaction
//                 SqlCommand accountCommand = connection.CreateCommand();
//                 accountCommand.Transaction = sqlTran;

//                 SqlCommand koperCommand = connection.CreateCommand();
//                 koperCommand.Transaction = sqlTran;

//                 try
//                 {
//                     // --- Step 1: INSERT into Account and get the new ID ---
//                     accountCommand.CommandText = insertAccountSql;
//                     accountCommand.Parameters.AddWithValue("@email", newKoper.Email);
//                     accountCommand.Parameters.AddWithValue("@password", newKoper.Password);

//                     newAccountId = Convert.ToInt32(accountCommand.ExecuteScalar());

//                     // --- Step 2: INSERT into Koper using the new Account ID ---
//                     koperCommand.CommandText = insertKoperSql;
//                     koperCommand.Parameters.AddWithValue("@accountId", newAccountId);
//                     koperCommand.Parameters.AddWithValue("@firstName", newKoper.FirstName);
//                     koperCommand.Parameters.AddWithValue("@lastName", newKoper.LastName);
//                     koperCommand.Parameters.AddWithValue("@address", newKoper.Address);
//                     koperCommand.Parameters.AddWithValue("@postCode", newKoper.PostCode);
//                     koperCommand.Parameters.AddWithValue("@regio", newKoper.Regio);

//                     koperCommand.ExecuteNonQuery();

//                     sqlTran.Commit(); // COMMIT if both queries succeeded
//                     return $"SUCCESS: Koper Account created with Account ID: {newAccountId}";
//                 }
//                 catch (Exception ex)
//                 {
//                     sqlTran.Rollback(); // ROLLBACK if an error occurred
//                     return $"FAILURE: Account creation failed and transaction was rolled back. Error: {ex.Message}";
//                 }
//             }
//         }

//         // --------------------------------------------------------------------------------
//         // 3. Get Koper Account (New method using DataReader)
//         // --------------------------------------------------------------------------------

//         [HttpGet]
//         [Route("koper/{accountId}")]
//         public KoperDetails GetKoperAccount(int accountId)
//         {
//             const string sql =
//                 "SELECT a.email, k.first_name, k.last_name, k.adress, k.regio " +
//                 "FROM Koper k JOIN Account a ON k.account_id = a.id " +
//                 "WHERE k.account_id = @id;";

//             KoperDetails details = null;

//             using (SqlConnection connection = new SqlConnection(connectionString))
//             {
//                 using (SqlCommand command = new SqlCommand(sql, connection))
//                 {
//                     command.Parameters.AddWithValue("@id", accountId);

//                     try
//                     {
//                         connection.Open();

//                         using (SqlDataReader reader = command.ExecuteReader())
//                         {
//                             if (reader.Read())
//                             {
//                                 details = new KoperDetails
//                                 {
//                                     AccountId = accountId,
//                                     Email = reader.GetString(0),
//                                     FirstName = reader.GetString(1),
//                                     LastName = reader.GetString(2),
//                                     Address = reader.GetString(3),
//                                     Regio = reader.GetString(4)
//                                 };
//                             }
//                         }
//                     }
//                     catch (Exception ex)
//                     {
//                         throw new Exception($"Database retrieval failed: {ex.Message}");
//                     }
//                 }
//             }

//             // Return the details object (or throw if not found)
//             return details ?? throw new KeyNotFoundException($"Koper account with ID {accountId} not found.");
//         }
//     }
// }

// // --------------------------------------------------------------------------------
// // Helper Classes (MUST be public to be used by public controller methods)
// // --------------------------------------------------------------------------------

// public class NewKoperAccount
// {
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public string Email { get; set; }
//     public string Password { get; set; }
//     public string Address { get; set; }
//     public string PostCode { get; set; }
//     public string Regio { get; set; }
// }

// public class KoperDetails
// {
//     public int AccountId { get; set; }
//     public string Email { get; set; }
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public string Address { get; set; }
//     public string Regio { get; set; }
// }