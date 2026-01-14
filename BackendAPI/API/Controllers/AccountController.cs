using API.Models;
using API.Utils;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("country/region")]
    public async Task<IActionResult> GetRegions()
    {
        var command = new GetRegionsCommand("Netherlands");
        var result = await _mediator.Send(command);
        return HttpSuccess<List<string>>.Ok(result, "Regions retrieved successfully");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] RequestLoginDTO loginRequest)
    {
        var command = new LoginAccountCommand(loginRequest);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Login successful");
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var (accountId, accountType) = GetUserClaim.GetInfo(User);

        var command = new GetAccountCommand(accountId, accountType);
        var result = await _mediator.Send(command);
        return HttpSuccess<object>.Ok(result, "Account information retrieved successfully");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new LogoutAccountCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Logout successful");
    }

    [HttpGet("reauthenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Reauthenticate()
    {
        var command = new ReauthenticateTokenCommand();
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Reauthentication successful");
    }

    [HttpGet("revoke-devices")]
    public async Task<IActionResult> RevokeDevices()
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new RevokeTokensCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("All devices revoked successfully");
    }

    // Admin endpoints
    [Tags("Admin")]
    [HttpPost("admin/create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDTO adminRequest)
    {
        var command = new CreateAdminCommand(adminRequest);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Created(result, "Admin account created successfully");
    }

    [Tags("Admin")]
    [HttpGet("admin/accounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var (_, accountType) = GetUserClaim.GetInfo(User);

        // Only allow admin users to access this endpoint
        if (accountType != AccountType.Admin)
            return HttpError.Forbidden("Only admin users can access this endpoint");

        var command = new GetAllAccountsCommand();
        var result = await _mediator.Send(command);
        return HttpSuccess<List<AccountListItemDto>>.Ok(result, "Accounts retrieved successfully");
    }

    [Tags("Admin")]
    [HttpDelete("admin/{accountId}")]
    public async Task<IActionResult> DeleteAccount(Guid accountId, [FromQuery] bool hardDelete = false)
    {
        var (_, accountType) = GetUserClaim.GetInfo(User);

        // Only allow admin users to access this endpoint
        if (accountType != AccountType.Admin)
            return HttpError.Forbidden("Only admin users can access this endpoint");

        var deleteRequest = new DeleteAccountRequestDTO
        {
            AccountId = accountId,
            HardDelete = hardDelete
        };
        var command = new DeleteAccountCommand(deleteRequest);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent(
            hardDelete ? "Account permanently deleted" : "Account soft deleted"
        );
    }

    [Tags("Admin")]
    [HttpPatch("admin/{accountId}/reactivate")]
    public async Task<IActionResult> ReactivateAccount(Guid accountId)
    {
        var (_, accountType) = GetUserClaim.GetInfo(User);

        // Only allow admin users to access this endpoint
        if (accountType != AccountType.Admin)
            return HttpError.Forbidden("Only admin users can access this endpoint");

        var command = new ReactivateAccountCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Account reactivated successfully");
    }
}