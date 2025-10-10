using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace WebProject_Klas1_Groep2.Data;
using WebProject_Klas1_Groep2.Models;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly VeilingKlokContext _db;

    public TestController(VeilingKlokContext db)
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
}