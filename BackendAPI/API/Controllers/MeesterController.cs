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

[Authorize(Roles = nameof(AccountType.Veilingmeester))]
[ApiController]
[Route("api/account/meester")]
public class MeesterController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeesterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateMeesterAccount([FromBody] CreateMeesterDTO account)
    {
        var command = new CreateMeesterCommand(account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Meester account created successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateMeesterAccount(
        [FromBody] UpdateVeilingMeesterDTO account
    )
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateVeilingmeesterCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Meester account updated successfully");
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

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var query = new GetOrderCommand(orderId, accountId);
        var result = await _mediator.Send(query);
        return HttpSuccess<OrderDetailsOutputDto>.Ok(result);
    }

    [HttpPut("order/{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromQuery] OrderStatus status)
    {
        var command = new UpdateOrderStatusCommand(orderId, status);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Order status updated successfully");
    }

    [HttpPost("veilingklok")]
    public async Task<IActionResult> CreateVeilingKlok([FromBody] CreateVeilingKlokDTO veiling)
    {
        var (meesterId, _) = GetUserClaim.GetInfo(User);
        var command = new CreateVeilingKlokCommand(veiling, meesterId);
        var result = await _mediator.Send(command);
        return HttpSuccess<VeilingKlokDetailsOutputDto>.Created(
            result,
            "VeilingKlok created successfully"
        );
    }

    [HttpGet("veilingklok/{klokId}")]
    public async Task<IActionResult> GetVeilingKlok(Guid klokId)
    {
        var command = new GetVeilingKlokCommand(klokId);
        var result = await _mediator.Send(command);
        return HttpSuccess<VeilingKlokOutputDto>.Ok(result);
    }

    [HttpGet("product/{productId}/details")]
    public async Task<IActionResult> GetProductDetails(Guid productId)
    {
        var command = new GetProductDetailsCommand(productId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(result);
    }

    [HttpPut("product/{productId}/price")]
    public async Task<IActionResult> UpdateProductPrice(
        Guid productId,
        [FromQuery] Guid kwekerId,
        [FromBody] UpdateProductDTO product
    )
    {
        var command = new UpdateProductCommand(productId, product, kwekerId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(
            result,
            "Product price updated successfully"
        );
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? nameFilter,
        [FromQuery] string? regionFilter,
        [FromQuery] decimal? maxPrice,
        [FromQuery] Guid? kwekerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var query = new GetProductsQuery(nameFilter, regionFilter, maxPrice, kwekerId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return HttpSuccess<PaginatedOutputDto<ProductOutputDto>>.Ok(result);
    }

    [HttpPost("veilingklok/{klokId}/start/{productId}")]
    public async Task<IActionResult> StartVeilingProduct(Guid klokId, Guid productId)
    {
        var command = new StartVeilingProductCommand(klokId, productId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Veiling product started successfully");
    }

    [HttpPut("veilingklok/{klokId}/status")]
    public async Task<IActionResult> UpdateVeilingKlokStatus(
        Guid klokId,
        [FromQuery] VeilingKlokStatus status
    )
    {
        var command = new UpdateVeilingKlokStatusCommand(klokId, status);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("VeilingKlok status updated successfully");
    }
}