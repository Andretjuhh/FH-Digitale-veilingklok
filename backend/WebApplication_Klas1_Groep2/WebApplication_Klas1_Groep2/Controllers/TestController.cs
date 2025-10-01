using Microsoft.AspNetCore.Mvc;

namespace WebApplication_Klas1_Groep2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Hello from the API!");
        }
    }
}
