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

[Authorize(Roles = nameof(AccountType.Veilingmeester))]
[ApiController]
[Route("api/account/meester")]
public class MeesterController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignInManager<Domain.Entities.Account> _signInManager;

    public MeesterController(
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
        [FromBody] CreateMeesterDTO account,
        [FromQuery] bool useCookies = true,
        [FromQuery] bool? useSessionCookies = null
    )
    {
        var command = new CreateMeesterCommand(account);
        var meester = await _mediator.Send(command);

        // Sign in the user automatically after creation
        if (useCookies)
        {
            var isPersistent = useSessionCookies != true;
            await _signInManager.SignInAsync(meester, isPersistent);
        }

        return HttpSuccess<Guid>.Created(
            meester.Id,
            "Veilingmeester account created and authenticated successfully"
        );
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateMeesterAccount(
        [FromBody] UpdateVeilingMeesterDTO account
    )
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateVeilingmeesterCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Meester account updated successfully");
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
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var query = new GetOrderCommand(orderId, accountId);
        var result = await _mediator.Send(query);
        return HttpSuccess<OrderDetailsOutputDto>.Ok(result);
    }

    [HttpPost("order/{orderId}/status")]
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

    [HttpGet("veilingklok/{klokId}/products")]
    public async Task<IActionResult> GetVeilingKlokProducts(Guid klokId)
    {
        var query = new GetVeilingKlokProductsQuery(klokId);
        var result = await _mediator.Send(query);
        return HttpSuccess<List<ProductOutputDto>>.Ok(result);
    }

    [HttpGet("veilingklok/{klokId}/orders")]
    public async Task<IActionResult> GetVeilingKlokOrders(
        Guid klokId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? beforeDate,
        [FromQuery] DateTime? afterDate
    )
    {
        var query = new GetVeilingKlokOrdersQuery(klokId, status, beforeDate, afterDate);
        var result = await _mediator.Send(query);
        return HttpSuccess<List<OrderOutputDto>>.Ok(result);
    }

    [HttpGet("product/{productId}/details")]
    public async Task<IActionResult> GetProductDetails(Guid productId)
    {
        var command = new GetProductDetailsCommand(productId);
        var result = await _mediator.Send(command);
        return HttpSuccess<ProductDetailsOutputDto>.Ok(result);
    }

    [HttpPost("product/{productId}/price")]
    public async Task<IActionResult> UpdateProductPrice(Guid productId, [FromQuery] decimal price)
    {
        var command = new UpdateProductAuctionPriceCommand(productId, price);
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
        [FromQuery] Guid? klokId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var query = new GetProductsQuery(
            nameFilter,
            regionFilter,
            maxPrice,
            kwekerId,
            klokId,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(query);
        return HttpSuccess<PaginatedOutputDto<ProductOutputDto>>.Ok(result);
    }

    [HttpGet("veilingklokken")]
    public async Task<IActionResult> GetVeilingKlokken(
        [FromQuery] VeilingKlokStatus? statusFilter,
        [FromQuery] string? region,
        [FromQuery] DateTime? scheduledAfter,
        [FromQuery] DateTime? scheduledBefore,
        [FromQuery] DateTime? startedAfter,
        [FromQuery] DateTime? startedBefore,
        [FromQuery] DateTime? endedAfter,
        [FromQuery] DateTime? endedBefore,
        [FromQuery] Guid? meesterId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var query = new GetVeilingKlokkenQuery(
            statusFilter,
            region,
            scheduledAfter,
            scheduledBefore,
            startedAfter,
            startedBefore,
            endedAfter,
            endedBefore,
            meesterId,
            pageNumber,
            pageSize
        );
        var result = await _mediator.Send(query);
        return HttpSuccess<PaginatedOutputDto<VeilingKlokOutputDto>>.Ok(result);
    }

    [HttpPost("veilingklok/{klokId}/start/{productId}")]
    public async Task<IActionResult> StartVeilingProduct(Guid klokId, Guid productId)
    {
        var command = new StartVeilingProductCommand(klokId, productId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Veiling product started successfully");
    }

    [HttpPost("veilingklok/{klokId}/product/{productId}")]
    public async Task<IActionResult> AddProductToVeilingKlok(
        Guid klokId,
        Guid productId,
        [FromQuery] decimal auctionPrice
    )
    {
        var command = new AddProductToVeilingKlokCommand(klokId, productId, auctionPrice);
        await _mediator.Send(command);
        return HttpSuccess<string>.Ok("Product added to VeilingKlok successfully");
    }

    [HttpGet("veilingklok/{klokId}/product/{productId}")]
    public async Task<IActionResult> RemoveProductFromVeilingKlok(Guid klokId, Guid productId)
    {
        var command = new RemoveProductFromVeilingKlokCommand(klokId, productId);
        await _mediator.Send(command);
        return HttpSuccess<string>.Ok("Product removed from VeilingKlok successfully");
    }

    [HttpPost("veilingklok/{klokId}/status")]
    public async Task<IActionResult> UpdateVeilingKlokStatus(
        Guid klokId,
        [FromQuery] VeilingKlokStatus status
    )
    {
        var command = new UpdateVeilingKlokStatusCommand(klokId, status);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("VeilingKlok status updated successfully");
    }

    [HttpGet("veilingklok/{klokId}/delete")]
    public async Task<IActionResult> DeleteVeilingKlok(Guid klokId)
    {
        var command = new DeleteVeilingKlokCommand(klokId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("VeilingKlok deleted successfully");
    }
}
