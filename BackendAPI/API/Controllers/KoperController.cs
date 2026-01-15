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

[Authorize(Roles = nameof(AccountType.Koper))]
[ApiController]
[Route("api/account/koper")]
public class KoperController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignInManager<Domain.Entities.Account> _signInManager;

    public KoperController(IMediator mediator, SignInManager<Domain.Entities.Account> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateKoperDTO account,
        [FromQuery] bool useCookies = true,
        [FromQuery] bool? useSessionCookies = null
    )
    {
        var command = new CreateKoperCommand(account);
        var koper = await _mediator.Send(command);

        // Sign in the user automatically after creation
        if (useCookies)
        {
            var isPersistent = useSessionCookies != true;
            await _signInManager.SignInAsync(koper, isPersistent);
        }

        return HttpSuccess<Guid>.Created(
            koper.Id,
            "Koper account created and authenticated successfully"
        );
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

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO order)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new CreateOrderCommand(accountId, order);
        var result = await _mediator.Send(command);
        return HttpSuccess<OrderOutputDto>.Created(result, "Order created successfully");
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var query = new GetOrderCommand(orderId, accountId);
        var result = await _mediator.Send(query);
        return HttpSuccess<OrderDetailsOutputDto>.Ok(result);
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
}
