using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;

namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for managing VeilingKlok (Auction Clock) operations
    /// Handles CRUD operations for auction clocks in the system
    /// </summary>
    [ApiController]
    [Route("api/veilingklok")]
    public class VeilingKlokController : ControllerBase
    {
        private readonly VeilingKlokContext _db;

        public VeilingKlokController(VeilingKlokContext db)
        {
            _db = db;
        }

        #region Create VeilingKlok

        /// <summary>
        /// Creates a new VeilingKlok (Auction Clock)
        /// </summary>
        /// <param name="newVeilingKlok">VeilingKlok details to create</param>
        /// <returns>Created VeilingKlok details with HTTP 201 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateVeilingKlok([FromBody] NewVeilingKlok newVeilingKlok)
        {
            // Validate input payload
            if (newVeilingKlok == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "VeilingKlok data is required",
                    400
                );
                return BadRequest(error);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(newVeilingKlok.Naam))
            {
                var error = new HtppError(
                    "Validation Error",
                    "VeilingKlok name (Naam) is required",
                    400
                );
                return BadRequest(error);
            }

            if (newVeilingKlok.DurationInSeconds <= 0)
            {
                var error = new HtppError(
                    "Validation Error",
                    "Duration must be greater than zero",
                    400
                );
                return BadRequest(error);
            }

            // Validate time range
            if (newVeilingKlok.EndTime <= newVeilingKlok.StartTime)
            {
                var error = new HtppError(
                    "Validation Error",
                    "End time must be after start time",
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Verify that the Veilingmeester exists using LINQ
                var veilingmeesterExists = await _db.Veilingmeesters
                    .AnyAsync(v => v.Id == newVeilingKlok.VeilingmeesterId);

                if (!veilingmeesterExists)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Veilingmeester with ID {newVeilingKlok.VeilingmeesterId} does not exist",
                        404
                    );
                    return NotFound(error);
                }

                // Create the VeilingKlok entity
                var veilingKlok = new VeilingKlok
                {
                    Naam = newVeilingKlok.Naam,
                    DurationInSeconds = newVeilingKlok.DurationInSeconds,
                    StartTime = newVeilingKlok.StartTime,
                    EndTime = newVeilingKlok.EndTime,
                    VeilingmeesterId = newVeilingKlok.VeilingmeesterId,
                    LiveViews = 0 // Initialize with 0 views
                };

                _db.Veilingklokken.Add(veilingKlok);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO
                var veilingKlokDetails = new VeilingKlokDetails
                {
                    Id = veilingKlok.Id,
                    Naam = veilingKlok.Naam,
                    DurationInSeconds = veilingKlok.DurationInSeconds,
                    LiveViews = veilingKlok.LiveViews,
                    StartTime = veilingKlok.StartTime,
                    EndTime = veilingKlok.EndTime,
                    VeilingmeesterId = veilingKlok.VeilingmeesterId
                };

                return CreatedAtAction(
                    nameof(GetVeilingKlok),
                    new { veilingKlokId = veilingKlok.Id },
                    new { message = "VeilingKlok created successfully", veilingKlok = veilingKlokDetails }
                );
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
                    $"VeilingKlok creation failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get VeilingKlok(s)

        /// <summary>
        /// Retrieves a specific VeilingKlok by ID
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok to retrieve</param>
        /// <returns>VeilingKlok details with HTTP 200 status</returns>
        [HttpGet("{veilingKlokId}")]
        public async Task<ActionResult<VeilingKlokDetails>> GetVeilingKlok(int veilingKlokId)
        {
            try
            {
                // Retrieve VeilingKlok using LINQ with Select for DTO mapping
                var veilingKlok = await _db.Veilingklokken
                    .Where(v => v.Id == veilingKlokId)
                    .Select(v => new VeilingKlokDetails
                    {
                        Id = v.Id,
                        Naam = v.Naam,
                        DurationInSeconds = v.DurationInSeconds,
                        LiveViews = v.LiveViews,
                        StartTime = v.StartTime,
                        EndTime = v.EndTime,
                        VeilingmeesterId = v.VeilingmeesterId
                    })
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"VeilingKlok with ID {veilingKlokId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(veilingKlok);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve VeilingKlok: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        /// <summary>
        /// Retrieves all VeilingKlokken, optionally filtered by Veilingmeester ID
        /// </summary>
        /// <param name="veilingmeesterId">Optional filter by Veilingmeester ID</param>
        /// <param name="activeOnly">Optional filter for active auctions (current time between start and end)</param>
        /// <returns>List of VeilingKlokken with HTTP 200 status</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<VeilingKlokDetails>>> GetAllVeilingKlokken(
            [FromQuery] int? veilingmeesterId = null,
            [FromQuery] bool activeOnly = false)
        {
            try
            {
                // Build query using LINQ
                var query = _db.Veilingklokken.AsQueryable();

                // Apply filter if veilingmeesterId is provided
                if (veilingmeesterId.HasValue)
                {
                    query = query.Where(v => v.VeilingmeesterId == veilingmeesterId.Value);
                }

                // Apply active filter if requested
                if (activeOnly)
                {
                    var currentTime = DateTime.UtcNow;
                    query = query.Where(v => v.StartTime <= currentTime && v.EndTime >= currentTime);
                }

                // Project to DTO using LINQ Select
                var veilingKlokken = await query
                    .Select(v => new VeilingKlokDetails
                    {
                        Id = v.Id,
                        Naam = v.Naam,
                        DurationInSeconds = v.DurationInSeconds,
                        LiveViews = v.LiveViews,
                        StartTime = v.StartTime,
                        EndTime = v.EndTime,
                        VeilingmeesterId = v.VeilingmeesterId
                    })
                    .OrderBy(v => v.StartTime) // Order by start time
                    .ToListAsync();

                return Ok(new { count = veilingKlokken.Count, veilingKlokken });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve VeilingKlokken: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Update VeilingKlok

        /// <summary>
        /// Updates an existing VeilingKlok
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok to update</param>
        /// <param name="updateVeilingKlok">Updated VeilingKlok details</param>
        /// <returns>Updated VeilingKlok details with HTTP 200 status</returns>
        [HttpPut("update/{veilingKlokId}")]
        public async Task<IActionResult> UpdateVeilingKlok(int veilingKlokId, [FromBody] UpdateVeilingKlok updateVeilingKlok)
        {
            // Validate input payload
            if (updateVeilingKlok == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Update data is required",
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db.Veilingklokken
                    .Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"VeilingKlok with ID {veilingKlokId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateVeilingKlok.Naam))
                {
                    veilingKlok.Naam = updateVeilingKlok.Naam;
                }

                if (updateVeilingKlok.DurationInSeconds.HasValue)
                {
                    if (updateVeilingKlok.DurationInSeconds.Value <= 0)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Validation Error",
                            "Duration must be greater than zero",
                            400
                        );
                        return BadRequest(error);
                    }
                    veilingKlok.DurationInSeconds = updateVeilingKlok.DurationInSeconds.Value;
                }

                if (updateVeilingKlok.StartTime.HasValue)
                {
                    veilingKlok.StartTime = updateVeilingKlok.StartTime.Value;
                }

                if (updateVeilingKlok.EndTime.HasValue)
                {
                    veilingKlok.EndTime = updateVeilingKlok.EndTime.Value;
                }

                // Validate time range after updates
                if (veilingKlok.EndTime <= veilingKlok.StartTime)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Validation Error",
                        "End time must be after start time",
                        400
                    );
                    return BadRequest(error);
                }

                if (updateVeilingKlok.LiveViews.HasValue)
                {
                    if (updateVeilingKlok.LiveViews.Value < 0)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Validation Error",
                            "Live views cannot be negative",
                            400
                        );
                        return BadRequest(error);
                    }
                    veilingKlok.LiveViews = updateVeilingKlok.LiveViews.Value;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO
                var veilingKlokDetails = new VeilingKlokDetails
                {
                    Id = veilingKlok.Id,
                    Naam = veilingKlok.Naam,
                    DurationInSeconds = veilingKlok.DurationInSeconds,
                    LiveViews = veilingKlok.LiveViews,
                    StartTime = veilingKlok.StartTime,
                    EndTime = veilingKlok.EndTime,
                    VeilingmeesterId = veilingKlok.VeilingmeesterId
                };

                return Ok(new { message = "VeilingKlok updated successfully", veilingKlok = veilingKlokDetails });
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
                    $"VeilingKlok update failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Delete VeilingKlok

        /// <summary>
        /// Deletes a VeilingKlok by ID
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok to delete</param>
        /// <returns>Success message with HTTP 200 status</returns>
        [HttpDelete("delete/{veilingKlokId}")]
        public async Task<IActionResult> DeleteVeilingKlok(int veilingKlokId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db.Veilingklokken
                    .Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"VeilingKlok with ID {veilingKlokId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Remove the VeilingKlok
                _db.Veilingklokken.Remove(veilingKlok);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"VeilingKlok with ID {veilingKlokId} deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Database Error",
                    $"Database constraint violation (VeilingKlok may be referenced elsewhere): {ex.InnerException?.Message ?? ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Internal Server Error",
                    $"VeilingKlok deletion failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Increment Live Views

        /// <summary>
        /// Increments the live views counter for a VeilingKlok
        /// Useful for tracking real-time auction engagement
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok</param>
        /// <returns>Updated live views count with HTTP 200 status</returns>
        [HttpPatch("{veilingKlokId}/increment-views")]
        public async Task<IActionResult> IncrementLiveViews(int veilingKlokId)
        {
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db.Veilingklokken
                    .Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"VeilingKlok with ID {veilingKlokId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Increment live views
                veilingKlok.LiveViews++;
                await _db.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Live views incremented successfully", 
                    veilingKlokId = veilingKlok.Id,
                    liveViews = veilingKlok.LiveViews 
                });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to increment live views: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion
    }
}
