using API.Models;
using API.Utils;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;

    public AccountController(
        IMediator mediator,
        UserManager<Account> userManager,
        SignInManager<Account> signInManager
    )
    {
        _mediator = mediator;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("country/region")]
    public async Task<IActionResult> GetRegions()
    {
        var command = new GetRegionsCommand("Netherlands");
        var result = await _mediator.Send(command);
        return HttpSuccess<List<string>>.Ok(result, "Regions retrieved successfully");
    }

    // Login is now handled by Identity API Endpoints mapped in Program.cs
    // POST /api/account/login

    [HttpGet("info")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var command = new GetAccountCommand(user.Id, user.AccountType);
        var result = await _mediator.Send(command);

        return HttpSuccess<object>.Ok(result, "Account information retrieved successfully");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return HttpSuccess<string>.NoContent("Logout successful");
    }

    [HttpGet("revoke-devices")]
    public async Task<IActionResult> RevokeDevices()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            // Identity doesn't strictly have "revoke devices" same as refresh tokens list unless we implement it.
            // But we can update SecurityStamp to invalidate all cookies/tokens.
            await _userManager.UpdateSecurityStampAsync(user);
        }
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
    public async Task<IActionResult> DeleteAccount(
        Guid accountId,
        [FromQuery] bool hardDelete = false
    )
    {
        var (currentAccountId, accountType) = GetUserClaim.GetInfo(User);

        // Only allow admin users to access this endpoint
        if (accountType != AccountType.Admin)
            return HttpError.Forbidden("Only admin users can access this endpoint");

        // Prevent admin from deleting themselves
        if (currentAccountId == accountId)
            return HttpError.BadRequest("You cannot delete your own account");

        var deleteRequest = new DeleteAccountRequestDTO
        {
            AccountId = accountId,
            HardDelete = hardDelete,
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
