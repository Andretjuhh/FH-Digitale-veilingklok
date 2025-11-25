using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;

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
        private readonly VeilingKlokContext _db;

        public ProductController(VeilingKlokContext db)
        {
            _db = db;
        }

        #region Create Product

        /// <summary>
        /// Creates a new product for a kweker (grower)
        /// </summary>
        /// <param name="newProduct">Product details to create</param>
        /// <returns>Created product details with HTTP 201 status</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] NewProduct newProduct)
        {
            // Validate input payload
            if (newProduct == null)
            {
                var error = new HtppError(
                    "Bad Request",
                    "Product data is required",
                    400
                );
                return BadRequest(error);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(newProduct.Name))
            {
                var error = new HtppError(
                    "Validation Error",
                    "Product name is required",
                    400
                );
                return BadRequest(error);
            }

            if (newProduct.Price <= 0)
            {
                var error = new HtppError(
                    "Validation Error",
                    "Product price must be greater than zero",
                    400
                );
                return BadRequest(error);
            }

            if (newProduct.MinimumPrice < 0)
            {
                var error = new HtppError(
                    "Validation Error",
                    "Minimum price cannot be negative",
                    400
                );
                return BadRequest(error);
            }

            if (newProduct.Quantity < 0)
            {
                var error = new HtppError(
                    "Validation Error",
                    "Quantity cannot be negative",
                    400
                );
                return BadRequest(error);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Verify that the Kweker exists using LINQ
                var kwekerExists = await _db.Kwekers
                    .AnyAsync(k => k.Id == newProduct.KwekerId);

                if (!kwekerExists)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Kweker with ID {newProduct.KwekerId} does not exist",
                        404
                    );
                    return NotFound(error);
                }

                // Create the product entity
                var product = new Product
                {
                    Name = newProduct.Name,
                    Description = newProduct.Description,
                    Price = newProduct.Price,
                    MinimumPrice = newProduct.MinimumPrice,
                    Quantity = newProduct.Quantity,
                    ImageUrl = newProduct.ImageUrl,
                    Size = newProduct.Size,
                    KwekerId = newProduct.KwekerId
                };

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO using LINQ Select
                var productDetails = new ProductDetails
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    MinimumPrice = product.MinimumPrice,
                    Quantity = product.Quantity,
                    ImageUrl = product.ImageUrl,
                    Size = product.Size,
                    KwekerId = product.KwekerId
                };

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { productId = product.Id },
                    new { message = "Product created successfully", product = productDetails }
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
                    $"Product creation failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Get Product(s)

        /// <summary>
        /// Retrieves a specific product by ID
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve</param>
        /// <returns>Product details with HTTP 200 status</returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductDetails>> GetProduct(int productId)
        {
            try
            {
                // Retrieve product using LINQ with Select for DTO mapping
                var product = await _db.Products
                    .Where(p => p.Id == productId)
                    .Select(p => new ProductDetails
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        MinimumPrice = p.MinimumPrice,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        Size = p.Size,
                        KwekerId = p.KwekerId
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    var error = new HtppError(
                        "Not Found",
                        $"Product with ID {productId} not found",
                        404
                    );
                    return NotFound(error);
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve product: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        /// <summary>
        /// Retrieves all products, optionally filtered by Kweker ID
        /// </summary>
        /// <param name="kwekerId">Optional filter by Kweker ID</param>
        /// <returns>List of products with HTTP 200 status</returns>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDetails>>> GetAllProducts([FromQuery] int? kwekerId = null)
        {
            try
            {
                // Build query using LINQ
                var query = _db.Products.AsQueryable();

                // Apply filter if kwekerId is provided
                if (kwekerId.HasValue)
                {
                    query = query.Where(p => p.KwekerId == kwekerId.Value);
                }

                // Project to DTO using LINQ Select
                var products = await query
                    .Select(p => new ProductDetails
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        MinimumPrice = p.MinimumPrice,
                        Quantity = p.Quantity,
                        ImageUrl = p.ImageUrl,
                        Size = p.Size,
                        KwekerId = p.KwekerId
                    })
                    .ToListAsync();

                return Ok(new { count = products.Count, products });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve products: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Update Product

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="productId">The ID of the product to update</param>
        /// <param name="updateProduct">Updated product details</param>
        /// <returns>Updated product details with HTTP 200 status</returns>
        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProduct updateProduct)
        {
            // Validate input payload
            if (updateProduct == null)
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
                // Retrieve the product using LINQ
                var product = await _db.Products
                    .Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Product with ID {productId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateProduct.Name))
                {
                    product.Name = updateProduct.Name;
                }

                if (updateProduct.Description != null)
                {
                    product.Description = updateProduct.Description;
                }

                if (updateProduct.Price.HasValue)
                {
                    if (updateProduct.Price.Value <= 0)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Validation Error",
                            "Product price must be greater than zero",
                            400
                        );
                        return BadRequest(error);
                    }
                    product.Price = updateProduct.Price.Value;
                }

                if (updateProduct.MinimumPrice.HasValue)
                {
                    if (updateProduct.MinimumPrice.Value < 0)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Validation Error",
                            "Minimum price cannot be negative",
                            400
                        );
                        return BadRequest(error);
                    }
                    product.MinimumPrice = updateProduct.MinimumPrice.Value;
                }

                if (updateProduct.Quantity.HasValue)
                {
                    if (updateProduct.Quantity.Value < 0)
                    {
                        await transaction.RollbackAsync();
                        var error = new HtppError(
                            "Validation Error",
                            "Quantity cannot be negative",
                            400
                        );
                        return BadRequest(error);
                    }
                    product.Quantity = updateProduct.Quantity.Value;
                }

                if (updateProduct.ImageUrl != null)
                {
                    product.ImageUrl = updateProduct.ImageUrl;
                }

                if (updateProduct.Size != null)
                {
                    product.Size = updateProduct.Size;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map to output DTO using LINQ Select
                var productDetails = new ProductDetails
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    MinimumPrice = product.MinimumPrice,
                    Quantity = product.Quantity,
                    ImageUrl = product.ImageUrl,
                    Size = product.Size,
                    KwekerId = product.KwekerId
                };

                return Ok(new { message = "Product updated successfully", product = productDetails });
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
                    $"Product update failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
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
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Retrieve the product using LINQ
                var product = await _db.Products
                    .Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    await transaction.RollbackAsync();
                    var error = new HtppError(
                        "Not Found",
                        $"Product with ID {productId} not found",
                        404
                    );
                    return NotFound(error);
                }

                // Remove the product
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"Product with ID {productId} deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Database Error",
                    $"Database constraint violation (product may be referenced elsewhere): {ex.InnerException?.Message ?? ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = new HtppError(
                    "Internal Server Error",
                    $"Product deletion failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion
    }
}
 