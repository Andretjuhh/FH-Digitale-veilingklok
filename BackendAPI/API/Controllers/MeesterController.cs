using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/account/meester")]
public class MeesterController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeesterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateMeesterAccount([FromBody] CreateMeesterDTO account)
    {
        var command = new CreateMeesterCommand(account);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Meester account created successfully");
    }
}