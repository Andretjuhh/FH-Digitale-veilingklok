using API.Models;
using API.Utils;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Application.UseCases.Order;
using Application.UseCases.Product;
using Application.UseCases.VeilingKlok;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = nameof(AccountType.Kweker))]
[ApiController]
[Route("api/account/kweker")]
public class KwekerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignInManager<Domain.Entities.Account> _signInManager;

    public KwekerController(
        IMediator mediator,
        SignInManager<Domain.Entities.Account> signInManager
    )
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateKwekerDTO account,
        [FromQuery] bool useCookies = true,
        [FromQuery] bool? useSessionCookies = null
    )
    {
        var command = new CreateKwekerCommand(account);
        var kweker = await _mediator.Send(command);

        // Sign in the user automatically after creation
        if (useCookies)
        {
            var isPersistent = useSessionCookies != true;
            await _signInManager.SignInAsync(kweker, isPersistent);
        }

        return HttpSuccess<Guid>.Created(
            kweker.Id,
            "Kweker account created and authenticated successfully"
        );
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateKwekerDTO account)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateKwekerCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Kweker account updated successfully");
    }

    [HttpPost("order/{orderId}/product/{productItemId}")]
    public async Task<IActionResult> UpdateOrderProduct(
        Guid orderId,
        Guid productItemId,
        [FromQuery] int quantity
    )
    {
        var command = new UpdateOrderProductCommand(orderId, productItemId, quantity);
        var result = await _mediator.Send(command);
        return HttpSuccess<OrderOutputDto>.Ok(result, "Order product updated successfully");
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var query = new GetKwekerOrderCommand(orderId, kwekerId);
        var result = await _mediator.Send(query);
        return HttpSuccess<OrderKwekerOutput>.Ok(result);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? productNameFilter,
        [FromQuery] string? koperNameFilter,
        [FromQuery] OrderStatus? statusFilter,
        [FromQuery] DateTime? beforeDate,
        [FromQuery] DateTime? afterDate,
        [FromQuery] Guid? productId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new GetKwekerOrdersCommand(
            kwekerId,
            productNameFilter,
            koperNameFilter,
            statusFilter,
            beforeDate,
            afterDate,
            productId,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(command);
        return HttpSuccess<PaginatedOutputDto<OrderKwekerOutput>>.Ok(result);
    }

    [HttpPost("order/{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromQuery] OrderStatus status)
    {
        var command = new UpdateOrderStatusCommand(orderId, status);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Order status updated successfully");
    }

    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO product)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new CreateProductCommand(product, accountId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(result, "Product created successfully");
    }

    [HttpGet("product/{productId}/details")]
    public async Task<IActionResult> GetProductDetails(Guid productId)
    {
        var command = new GetProductDetailsCommand(productId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(result);
    }

    [HttpGet("product/{productId}/orders")]
    public async Task<IActionResult> GetProductOrders(
        Guid productId,
        [FromQuery] OrderStatus? statusFilter,
        [FromQuery] DateTime? beforeDate,
        [FromQuery] DateTime? afterDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new GetProductOrdersCommand(
            productId,
            kwekerId,
            statusFilter,
            beforeDate,
            afterDate,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(command);
        return HttpSuccess<PaginatedOutputDto<OrderOutputDto>>.Ok(result);
    }

    [HttpPost("product/{productId}")]
    public async Task<IActionResult> UpdateProduct(
        Guid productId,
        [FromBody] UpdateProductDTO product
    )
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateProductCommand(productId, product, kwekerId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(result, "Product updated successfully");
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? nameFilter,
        [FromQuery] string? regionFilter,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var query = new GetProductsQuery(
            nameFilter,
            regionFilter,
            maxPrice,
            kwekerId,
            null,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(query);
        return HttpSuccess<PaginatedOutputDto<ProductOutputDto>>.Ok(result);
    }

    [HttpGet("veilingklok/{klokId}")]
    public async Task<IActionResult> GetVeilingKlok(Guid klokId)
    {
        var command = new GetVeilingKlokCommand(klokId);
        var result = await _mediator.Send(command);
        return HttpSuccess<VeilingKlokOutputDto>.Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetKwekerStats()
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new GetKwekerStatsCommand(kwekerId);
        var result = await _mediator.Send(command);
        return HttpSuccess<KwekerStatsOutputDto>.Ok(result);
    }

    [HttpGet("product-stats")]
    public async Task<IActionResult> GetKwekerProductStats()
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new GetKwekerProductStatsCommand(kwekerId);
        var result = await _mediator.Send(command);
        return HttpSuccess<KwekerProductStatsOutputDto>.Ok(result);
    }

    [HttpGet("order-stats")]
    public async Task<IActionResult> GetKwekerOrderStats()
    {
        var (kwekerId, _) = GetUserClaim.GetInfo(User);
        var command = new GetKwekerOrderStatsCommand(kwekerId);
        var result = await _mediator.Send(command);
        return HttpSuccess<KwekerOrderStatsOutputDto>.Ok(result);
    }
}