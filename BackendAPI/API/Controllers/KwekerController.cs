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
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = nameof(AccountType.Kweker))]
[ApiController]
[Route("api/account/kweker")]
public class KwekerController : ControllerBase
{
    private readonly IMediator _mediator;

    public KwekerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAccount([FromBody] CreateKwekerDTO account)
    {
        var command = new CreateKwekerCommand(account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Kweker account created successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateKwekerDTO account)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateKwekerCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Kweker account updated successfully");
    }

    [HttpPut("order/{orderId}/product/{productItemId}")]
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

    [HttpPut("order/{orderId}/status")]
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
        return HttpSuccess<(IEnumerable<OrderOutputDto> data, int totalCount)>.Ok(result);
    }

    [HttpPut("product/{productId}")]
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
        [FromQuery] decimal? maxPrice,
        [FromQuery] Guid? kwekerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var query = new GetProductsQuery(nameFilter, maxPrice, kwekerId, pageNumber, pageSize);
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
}