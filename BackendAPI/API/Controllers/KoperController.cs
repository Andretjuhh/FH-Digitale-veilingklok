using API.Models;
using API.Utils;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
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
        return HttpSuccess<AuthOutputDto>.Ok(result, "Koper account created successfully");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateKoperDTO account)
    {
        var (accountId, _) = GetUserClaim.GetInfo(User);
        var command = new UpdateKoperCommand(accountId, account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AccountOutputDto>.Ok(result, "Koper account updated successfully");
    }
}