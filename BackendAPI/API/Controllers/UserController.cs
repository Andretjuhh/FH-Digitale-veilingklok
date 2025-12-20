using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/account")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RequestLoginDTO loginRequest)
    {
        var command = new LoginAccountCommand(loginRequest);
        var result = await _mediator.Send(command);
        return HttpSuccess<AuthOutputDto>.Ok(result, "Login successful");
    }
}
