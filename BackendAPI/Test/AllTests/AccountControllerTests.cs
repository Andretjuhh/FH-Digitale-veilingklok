using System.Security.Claims;
using API.Controllers;
using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Fakes;
using Xunit;

namespace Test.Controllers;

public class AccountControllerTests
{
	private static AccountController CreateController(FakeMediator mediator)
	{
		var controller = new AccountController(mediator)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};

		return controller;
	}

	private static void SetUser(AccountController controller, Guid accountId, AccountType role)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, accountId.ToString()),
			new(ClaimTypes.Role, role.ToString())
		};

		var identity = new ClaimsIdentity(claims, "TestAuth");
		controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
	}

	[Fact]
	public async Task GetRegions_WhenCalled_ReturnsRegionsInHttpSuccess()
	{
		var mediator = new FakeMediator();
		mediator.RegisterResponse<GetRegionsCommand, List<string>>(new List<string> { "Region1", "Region2" });
		var controller = CreateController(mediator);

		var result = await controller.GetRegions();

		var success = Assert.IsType<HttpSuccess<List<string>>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Regions retrieved successfully", success.Message);
		Assert.Equal(new List<string> { "Region1", "Region2" }, success.Data);
	}

	[Fact]
	public async Task Login_WithValidCredentials_ReturnsAuthOutputInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var authResult = new AuthOutputDto
		{
			AccessToken = "token",
			AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
			AccountType = AccountType.Koper
		};
		mediator.RegisterResponse<LoginAccountCommand, AuthOutputDto>(authResult);
		var controller = CreateController(mediator);

		var loginDto = new RequestLoginDTO
		{
			Email = "test@example.com",
			Password = "password"
		};

		var result = await controller.Login(loginDto);

		var success = Assert.IsType<HttpSuccess<AuthOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Login successful", success.Message);
		Assert.Equal(authResult.AccessToken, success.Data!.AccessToken);
	}

	[Fact]
	public async Task GetAccountInfo_WithValidUserClaims_ReturnsAccountInfoInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var accountResult = new { Email = "user@example.com" };
		mediator.RegisterResponse<GetAccountCommand, object>(accountResult);

		var controller = CreateController(mediator);
		var accountId = Guid.NewGuid();
		SetUser(controller, accountId, AccountType.Koper);

		var result = await controller.GetAccountInfo();

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Account information retrieved successfully", success.Message);

		var sentCommand = Assert.IsType<GetAccountCommand>(mediator.LastRequest);
		Assert.Equal(accountId, sentCommand.AccountId);
		Assert.Equal(AccountType.Koper, sentCommand.AccountType);
	}

	[Fact]
	public async Task Logout_WithValidUserClaims_ReturnsNoContentHttpSuccess()
	{
		var mediator = new FakeMediator();
		var controller = CreateController(mediator);
		var accountId = Guid.NewGuid();
		SetUser(controller, accountId, AccountType.Koper);

		var result = await controller.Logout();

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status204NoContent, success.StatusCode);
		Assert.Equal("Logout successful", success.Message);

		var sentCommand = Assert.IsType<LogoutAccountCommand>(mediator.LastRequest);
		Assert.Equal(accountId, sentCommand.AccountId);
	}

	[Fact]
	public async Task Reauthenticate_WithValidToken_ReturnsNewAuthOutputInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var authResult = new AuthOutputDto
		{
			AccessToken = "newToken",
			AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(2),
			AccountType = AccountType.Kweker
		};
		mediator.RegisterResponse<ReauthenticateTokenCommand, AuthOutputDto>(authResult);
		var controller = CreateController(mediator);

		var result = await controller.Reauthenticate();

		var success = Assert.IsType<HttpSuccess<AuthOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Reauthentication successful", success.Message);
		Assert.Equal(authResult.AccessToken, success.Data!.AccessToken);
	}

	[Fact]
	public async Task RevokeDevices_WithValidUserClaims_RevokesAllDevicesAndReturnsNoContent()
	{
		var mediator = new FakeMediator();
		var controller = CreateController(mediator);
		var accountId = Guid.NewGuid();
		SetUser(controller, accountId, AccountType.Koper);

		var result = await controller.RevokeDevices();

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status204NoContent, success.StatusCode);
		Assert.Equal("All devices revoked successfully", success.Message);

		var sentCommand = Assert.IsType<RevokeTokensCommand>(mediator.LastRequest);
		Assert.Equal(accountId, sentCommand.AccountId);
	}
}
