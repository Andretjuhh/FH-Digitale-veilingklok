using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Attributes;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Mappers;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.InputDTOs;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Types;
using VeilingKlokApp.Utils;
using VeilingKlokKlas1Groep2.Declarations;

namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for managing Product operations
    /// Handles CRUD operations for products in the auction system
    /// </summary>
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _db;

        public ProductController(DatabaseContext db)
        {
            _db = db;
        }

        #region Get Product

        /// <summary>
        /// Retrieves a specific product by ID
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve</param>
        /// <returns>Product details with HTTP 200 status</returns>
        [HttpGet("{productId}")]
        [AuthorizeAccountType([AccountType.Veilingmeester])]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            try
            {
                // Get authenticated account id (set by AuthorizeAttribute)
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Authenticated account not found in request");
                }

                // Retrieve product using LINQ with Select for DTO mapping
                var product = await _db
                    .Products.Where(p =>
                        p.Id == productId && p.KwekerId == accountContext.Value.AccountId
                    )
                    .Select(ProductMapper.ProjectToDetails)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return HtppError.NotFound($"Product with ID {productId} not found");
                }

                return HttpSuccess<ProductDetails>.Ok(product);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieves all products, optionally filtered by Kweker ID
        /// </summary>
        /// <returns>List of products with HTTP 200 status</returns>
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                // Get authenticated account id (set by AuthorizeAttribute)
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Authenticated account not found in request");
                }

                // Retrieve products using LINQ with Select for DTO mapping
                var products = await _db
                    .Products.Where(p => p.KwekerId == accountContext.Value.AccountId)
                    .Select(ProductMapper.ProjectToDetails)
                    .ToListAsync();

                var result = new { count = products.Count, products };
                return HttpSuccess<object>.Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion


        #region Delete Product

        /// <summary>
        /// Deletes a product by ID
        /// </summary>
        /// <param name="productId">The ID of the product to delete</param>
        /// <returns>Success message with HTTP 200 status</returns>
        [HttpDelete("delete/{productId}")]
        [AuthorizeAccountType(AccountType.Kweker)]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Get authenticated account id (set by AuthorizeAttribute)
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Authenticated account not found in request");
                }

                // Retrieve the product using LINQ
                var product = await _db
                    .Products.Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    await transaction.RollbackAsync();
                    return HtppError.NotFound($"Product with ID {productId} not found");
                }

                // Remove the product
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = new { productId = productId };
                return HttpSuccess<object>.Ok(
                    result,
                    $"Product with ID {productId} deleted successfully"
                );
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return HtppError
                    .InternalServerError(
                        $"Database constraint violation (product may be referenced elsewhere): {ex.InnerException?.Message ?? ex.Message}"
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
    }
}
