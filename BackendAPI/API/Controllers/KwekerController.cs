using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/account/kweker")]
public class KwekerController : ControllerBase
{
    private readonly IMediator _mediator;

    public KwekerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateKwekerDTO account)
    {
        var command = new CreateKwekerCommand(account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Kweker account created successfully");
    }
}