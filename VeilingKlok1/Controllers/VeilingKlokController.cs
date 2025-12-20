using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Attributes;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.InputDTOs;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Utils;
using VeilingKlokApp.Mappers;
using VeilingKlokKlas1Groep2.Declarations;

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
        private readonly DatabaseContext _db;

        public VeilingKlokController(DatabaseContext db)
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
            // Validate model state (includes required field validation)
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Verify that the Veilingmeester exists using LINQ
                var veilingmeesterExists = await _db.Veilingmeesters.AnyAsync(v =>
                    v.Id == newVeilingKlok.VeilingmeesterId
                );

                if (!veilingmeesterExists)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound(
                        $"Veilingmeester with ID {newVeilingKlok.VeilingmeesterId} does not exist"
                    );
                }

                // Create the VeilingKlok entity
                var veilingKlok = VeilingKlokMapper.ToEntity(newVeilingKlok);

                _db.Veilingklokken.Add(veilingKlok);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO
                var veilingKlokDetails = VeilingKlokMapper.ToDetails(veilingKlok);

                return HttpSuccess<VeilingKlokDetails>.Created(
                    veilingKlokDetails,
                    "VeilingKlok created successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
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
        public async Task<IActionResult> GetVeilingKlok(Guid veilingKlokId)
        {
            try
            {
                // Retrieve VeilingKlok using LINQ with Select for DTO mapping
                var veilingKlok = await _db
                    .Veilingklokken.Where(v => v.Id == veilingKlokId)
                    .Select(VeilingKlokMapper.ProjectToDetails)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    return HtppError.NotFound($"VeilingKlok with ID {veilingKlokId} not found");
                }

                return HttpSuccess<VeilingKlokDetails>.Ok(
                    veilingKlok,
                    "VeilingKlok retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all VeilingKlokken, optionally filtered by Veilingmeester ID
        /// </summary>
        /// <param name="veilingmeesterId">Optional filter by Veilingmeester ID</param>
        /// <param name="activeOnly">Optional filter for active auctions (current time between start and end)</param>
        /// <returns>List of VeilingKlokken with HTTP 200 status</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVeilingKlokken(
            [FromQuery] Guid? veilingmeesterId = null,
            [FromQuery] bool activeOnly = false
        )
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
                    query = query.Where(v =>
                        v.StartTime <= currentTime && v.EndTime >= currentTime
                    );
                }

                // Project to DTO using LINQ Select
                var veilingKlokken = await query
                    .Select(VeilingKlokMapper.ProjectToDetails)
                    .OrderBy(v => v.StartTime) // Order by start time
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

        #region Update VeilingKlok

        /// <summary>
        /// Updates an existing VeilingKlok
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok to update</param>
        /// <param name="updateVeilingKlok">Updated VeilingKlok details</param>
        /// <returns>Updated VeilingKlok details with HTTP 200 status</returns>
        [HttpPut("update/{veilingKlokId}")]
        public async Task<IActionResult> UpdateVeilingKlok(
            Guid veilingKlokId,
            [FromBody] UpdateVeilingKlok updateVeilingKlok
        )
        {
            // Validate model state
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db
                    .Veilingklokken.Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound($"VeilingKlok with ID {veilingKlokId} not found");
                }

                // Update entity using Mapper
                VeilingKlokMapper.UpdateEntity(veilingKlok, updateVeilingKlok);

                // Validate time range after updates
                if (veilingKlok.EndTime <= veilingKlok.StartTime)
                {
                    await transaction.RollbackAsync();
                    return HtppError.BadRequest("End time must be after start time");
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO
                var veilingKlokDetails = VeilingKlokMapper.ToDetails(veilingKlok);

                return HttpSuccess<VeilingKlokDetails>.Ok(
                    veilingKlokDetails,
                    "VeilingKlok updated successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ControllerHelper.HandleException(ex);
            }
        }

                return HttpSuccess<VeilingKlokDetails>.Ok(
                    veilingKlokDetails,
                    "VeilingKlok updated successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return HtppError
                    .InternalServerError(
                        $"Database constraint violation: {ex.InnerException?.Message ?? ex.Message}"
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

        #region Delete VeilingKlok

        /// <summary>
        /// Deletes a VeilingKlok by ID
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok to delete</param>
        /// <returns>Success message with HTTP 200 status</returns>
        [HttpDelete("delete/{veilingKlokId}")]
        public async Task<IActionResult> DeleteVeilingKlok(Guid veilingKlokId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db
                    .Veilingklokken.Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound($"VeilingKlok with ID {veilingKlokId} not found");
                }

                // Remove the VeilingKlok
                _db.Veilingklokken.Remove(veilingKlok);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return HttpSuccess<object>.Ok(
                    new { message = $"VeilingKlok with ID {veilingKlokId} deleted successfully" },
                    $"VeilingKlok with ID {veilingKlokId} deleted successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return HtppError
                    .InternalServerError(
                        $"Database constraint violation (VeilingKlok may be referenced elsewhere): {ex.InnerException?.Message ?? ex.Message}"
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

        #region Increment Live Views

        /// <summary>
        /// Increments the live views counter for a VeilingKlok
        /// Useful for tracking real-time auction engagement
        /// </summary>
        /// <param name="veilingKlokId">The ID of the VeilingKlok</param>
        /// <returns>Updated live views count with HTTP 200 status</returns>
        [HttpPatch("{veilingKlokId}/increment-views")]
        public async Task<IActionResult> IncrementLiveViews(Guid veilingKlokId)
        {
            try
            {
                // Retrieve the VeilingKlok using LINQ
                var veilingKlok = await _db
                    .Veilingklokken.Where(v => v.Id == veilingKlokId)
                    .FirstOrDefaultAsync();

                if (veilingKlok == null)
                {
                    return HtppError.NotFound($"VeilingKlok with ID {veilingKlokId} not found");
                }

                // Increment live views
                veilingKlok.LiveViews++;
                await _db.SaveChangesAsync();

                return HttpSuccess<object>.Ok(
                    new
                    {
                        message = "Live views incremented successfully",
                        veilingKlokId = veilingKlok.Id,
                        liveViews = veilingKlok.LiveViews,
                    },
                    "Live views incremented successfully"
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
