using API.Models;
using API.Utils;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
    [Authorize]
    public async Task<IActionResult> GetRegions()
    {
        var command = new GetRegionsCommand("Netherlands");
        var result = await _mediator.Send(command);
        return HttpSuccess<List<string>>.Ok(result, "Regions retrieved successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestLoginDTO loginRequest)
    {
        var command = new LoginAccountCommand(loginRequest);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Login successful");
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> GetAccountInfo()
    {
        var (accountId, accountType) = GetUserClaim.GetInfo(User);

        var command = new GetAccountCommand(accountId, accountType);
        var result = await _mediator.Send(command);
        return HttpSuccess<object>.Ok(result, "Account information retrieved successfully");
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new LogoutAccountCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Logout successful");
    }

    [HttpGet("reauthenticate")]
    public async Task<IActionResult> Reauthenticate()
    {
        var command = new ReauthenticateTokenCommand();
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Reauthentication successful");
    }

    [HttpGet("revoke-devices")]
    [Authorize]
    public async Task<IActionResult> RevokeDevices()
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new RevokeTokensCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("All devices revoked successfully");
    }
}
