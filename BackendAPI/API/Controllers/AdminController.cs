using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;

    public AdminController(
        IMediator mediator,
        UserManager<Account> userManager,
        SignInManager<Account> signInManager
    )
    {
        _mediator = mediator;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAdmin(
        [FromBody] CreateAdminDTO adminRequest,
        [FromQuery] bool useCookies = true,
        [FromQuery] bool? useSessionCookies = null
    )
    {
        var command = new CreateAdminCommand(adminRequest);
        var admin = await _mediator.Send(command);

        // Sign in the user automatically after creation
        if (useCookies)
        {
            var isPersistent = useSessionCookies != true;
            await _signInManager.SignInAsync(admin, isPersistent);
        }

        return HttpSuccess<object>.Created(
            new { id = admin.Id, email = admin.Email },
            "Admin account created and authenticated successfully."
        );
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var command = new GetAllAccountsCommand();
        var result = await _mediator.Send(command);
        return HttpSuccess<List<AccountListItemDto>>.Ok(result, "Accounts retrieved successfully");
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(
        Guid accountId,
        [FromQuery] bool hardDelete = false
    )
    {
        var currentUser = await _userManager.GetUserAsync(User);

        // Prevent admin from deleting themselves
        if (currentUser?.Id == accountId)
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

    [HttpPatch("{accountId}/reactivate")]
    public async Task<IActionResult> ReactivateAccount(Guid accountId)
    {
        var command = new ReactivateAccountCommand(accountId);
        await _mediator.Send(command);
        return HttpSuccess<string>.NoContent("Account reactivated successfully");
    }
}
