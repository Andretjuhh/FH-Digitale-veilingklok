using System.Reflection;
using API.Controllers;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Test.Authorization;

public class AuthorizationAttributesTests
{
	[Fact]
	public void AccountController_HasAuthorizeAttribute_AndLoginAndReauthAllowAnonymous()
	{
		var controllerType = typeof(AccountController);

		var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
		Assert.NotNull(authorize);

		var loginMethod = controllerType.GetMethod("Login");
		var reauthMethod = controllerType.GetMethod("Reauthenticate");

		Assert.NotNull(loginMethod);
		Assert.NotNull(reauthMethod);

		Assert.NotNull(loginMethod!.GetCustomAttribute<AllowAnonymousAttribute>());
		Assert.NotNull(reauthMethod!.GetCustomAttribute<AllowAnonymousAttribute>());
	}

	[Fact]
	public void KoperController_RequiresKoperRole_AndCreateAccountIsAnonymous()
	{
		var controllerType = typeof(KoperController);

		var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
		Assert.NotNull(authorize);
		Assert.Equal(nameof(AccountType.Koper), authorize!.Roles);

		var createMethod = controllerType.GetMethod("CreateAccount");
		Assert.NotNull(createMethod);
		Assert.NotNull(createMethod!.GetCustomAttribute<AllowAnonymousAttribute>());
	}

	[Fact]
	public void KwekerController_RequiresKwekerRole_AndCreateAccountIsAnonymous()
	{
		var controllerType = typeof(KwekerController);

		var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
		Assert.NotNull(authorize);
		Assert.Equal(nameof(AccountType.Kweker), authorize!.Roles);

		var createMethod = controllerType.GetMethod("CreateAccount");
		Assert.NotNull(createMethod);
		Assert.NotNull(createMethod!.GetCustomAttribute<AllowAnonymousAttribute>());
	}

	[Fact]
	public void MeesterController_RequiresVeilingmeesterRole_AndCreateAccountIsAnonymous()
	{
		var controllerType = typeof(MeesterController);

		var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
		Assert.NotNull(authorize);
		Assert.Equal(nameof(AccountType.Veilingmeester), authorize!.Roles);

		var createMethod = controllerType.GetMethod("CreateMeesterAccount");
		Assert.NotNull(createMethod);
		Assert.NotNull(createMethod!.GetCustomAttribute<AllowAnonymousAttribute>());
	}
}
