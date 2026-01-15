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
}
