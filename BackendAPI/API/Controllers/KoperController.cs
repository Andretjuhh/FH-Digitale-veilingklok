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
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/account/koper")]
public class KoperController : ControllerBase
{
    private readonly IMediator _mediator;

    public KoperController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateKoperDTO account)
    {
        var command = new CreateKoperCommand(account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Created(result, "Koper account created successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateKoperDTO account)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateKoperCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Koper account updated successfully");
    }

    [HttpPost("address")]
    public async Task<IActionResult> CreateAddress([FromBody] AddressInputDto address)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new CreateAddressCommand(accountId, address);
        var result = await _mediator.Send(command);
        return HttpSuccess<AddressOutputDto>.Created(result, "Address created successfully");
    }

    [HttpPut("address/primary/{addressId}")]
    public async Task<IActionResult> UpdatePrimaryAddress(int addressId)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdatePrimaryAddressCommand(accountId, addressId);
        var result = await _mediator.Send(command);
        return HttpSuccess<AddressOutputDto>.Ok(result, "Primary address updated successfully");
    }

    [HttpPost("order")]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDTO order,
        [FromQuery] Guid veilingKlokId
    )
    {
        var command = new CreateOrderCommand(order, veilingKlokId);
        var result = await _mediator.Send(command);
        return HttpSuccess<OrderOutputDto>.Created(result, "Order created successfully");
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var query = new GetOrderQuery(orderId);
        var result = await _mediator.Send(query);
        return HttpSuccess<OrderOutputDto>.Ok(result);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? beforeDate,
        [FromQuery] DateTime? afterDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new GetOrdersCommand(
            accountId,
            status,
            beforeDate,
            afterDate,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(command);
        return HttpSuccess<PaginatedOutputDto<OrderOutputDto>>.Ok(result);
    }

    [HttpPost("order/{orderId}/product")]
    public async Task<IActionResult> OrderProduct(
        Guid orderId,
        [FromQuery] Guid productId,
        [FromQuery] int quantity
    )
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new CreateOrderProductCommand(accountId, orderId, productId, quantity);
        var result = await _mediator.Send(command);
        return HttpSuccess<OrderItemOutputDto>.Ok(result, "Product ordered successfully");
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var command = new GetProductCommand(productId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductOutputDto>.Ok(result);
    }

    [HttpGet("veilingklok/{klokId}")]
    public async Task<IActionResult> GetVeilingKlok(Guid klokId)
    {
        var command = new GetVeilingKlokCommand(klokId);
        var result = await _mediator.Send(command);
        return HttpSuccess<VeilingKlokOutputDto>.Ok(result);
    }
}