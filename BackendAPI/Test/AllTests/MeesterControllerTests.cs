using System.Security.Claims;
using API.Controllers;
using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.Account;
using Application.UseCases.Order;
using Application.UseCases.Product;
using Application.UseCases.VeilingKlok;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Fakes;
using Xunit;

namespace Test.Controllers;

public class MeesterControllerTests
{
	private static MeesterController CreateController(FakeMediator mediator)
	{
		var controller = new MeesterController(mediator)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			}
		};

		return controller;
	}

	private static void SetUser(MeesterController controller, Guid accountId)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, accountId.ToString()),
			new(ClaimTypes.Role, AccountType.Veilingmeester.ToString())
		};

		var identity = new ClaimsIdentity(claims, "TestAuth");
		controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
	}

	[Fact]
	public async Task CreateMeesterAccount_WithValidData_ReturnsAuthOutputInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var authResult = new AuthOutputDto
		{
			AccessToken = "token",
			AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
			AccountType = AccountType.Veilingmeester
		};
		mediator.RegisterResponse<CreateMeesterCommand, AuthOutputDto>(authResult);

		var controller = CreateController(mediator);

		var dto = new CreateMeesterDTO
		{
			Email = "meester@example.com",
			Password = "password",
			Region = "Region",
			CountryCode = "NL",
			AuthorisatieCode = "AUTH"
		};

		var result = await controller.CreateMeesterAccount(dto);

		var success = Assert.IsType<HttpSuccess<AuthOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Meester account created successfully", success.Message);
	}

	[Fact]
	public async Task UpdateMeesterAccount_WithVeilingmeesterClaims_ReturnsUpdatedAccountInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var accountResult = new AccountOutputDto
		{
			AccountType = AccountType.Veilingmeester,
			Email = "meester@example.com"
		};
		mediator.RegisterResponse<UpdateVeilingmeesterCommand, AccountOutputDto>(accountResult);

		var controller = CreateController(mediator);
		var accountId = Guid.NewGuid();
		SetUser(controller, accountId);

		var dto = new UpdateVeilingMeesterDTO
		{
			Email = "meester@example.com",
			Password = "password",
			Regio = "Region",
			AuthorisatieCode = "AUTH"
		};

		var result = await controller.UpdateMeesterAccount(dto);

		var success = Assert.IsType<HttpSuccess<AccountOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Meester account updated successfully", success.Message);
	}

	[Fact]
	public async Task UpdateOrderProduct_WithValidIds_ReturnsUpdatedOrderInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var orderResult = new OrderOutputDto
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTimeOffset.UtcNow,
			Status = OrderStatus.Open,
			TotalAmount = 10,
			TotalItems = 1
		};
		mediator.RegisterResponse<UpdateOrderProductCommand, OrderOutputDto>(orderResult);

		var controller = CreateController(mediator);

		var result = await controller.UpdateOrderProduct(Guid.NewGuid(), Guid.NewGuid(), 5);

		var success = Assert.IsType<HttpSuccess<OrderOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Order product updated successfully", success.Message);
	}

	[Fact]
	public async Task GetOrder_WithVeilingmeesterClaims_ReturnsOrderDetailsInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var details = new OrderDetailsOutputDto
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTimeOffset.UtcNow,
			Status = OrderStatus.Open,
			TotalAmount = 20,
			TotalItems = 2,
			Products = new List<OrderItemOutputDto>()
		};
		mediator.RegisterResponse<GetOrderCommand, OrderDetailsOutputDto>(details);

		var controller = CreateController(mediator);
		var accountId = Guid.NewGuid();
		SetUser(controller, accountId);

		var result = await controller.GetOrder(Guid.NewGuid());

		var success = Assert.IsType<HttpSuccess<OrderDetailsOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
	}

	[Fact]
	public async Task UpdateOrderStatus_WithValidStatus_ReturnsNoContentHttpSuccess()
	{
		var mediator = new FakeMediator();
		var controller = CreateController(mediator);

		var result = await controller.UpdateOrderStatus(Guid.NewGuid(), OrderStatus.Delivered);

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status204NoContent, success.StatusCode);
		Assert.Equal("Order status updated successfully", success.Message);
	}

	[Fact]
	public async Task CreateVeilingKlok_WithVeilingmeesterClaims_ReturnsCreatedVeilingKlokInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var klokDetails = new VeilingKlokDetailsOutputDto
		{
			Id = Guid.NewGuid(),
			Status = VeilingKlokStatus.Scheduled,
			PeakedLiveViews = 0,
			CreatedAt = DateTimeOffset.UtcNow,
			RegionOrState = "Region",
			Country = "NL",
			CurrentBids = 0,
			TotalProducts = 0,
			Products = new List<ProductDetailsOutputDto>()
		};
		mediator.RegisterResponse<CreateVeilingKlokCommand, VeilingKlokDetailsOutputDto>(klokDetails);

		var controller = CreateController(mediator);
		var meesterId = Guid.NewGuid();
		SetUser(controller, meesterId);

		var dto = new CreateVeilingKlokDTO
		{
			ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
			VeilingDurationMinutes = 30,
			Products = new Dictionary<Guid, decimal> { { Guid.NewGuid(), 1m } }
		};

		var result = await controller.CreateVeilingKlok(dto);

		var success = Assert.IsType<HttpSuccess<VeilingKlokDetailsOutputDto>>(result);
		Assert.Equal(StatusCodes.Status201Created, success.StatusCode);
		Assert.Equal("VeilingKlok created successfully", success.Message);
	}

	[Fact]
	public async Task GetVeilingKlok_WithValidId_ReturnsVeilingKlokInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var klok = new VeilingKlokOutputDto
		{
			Id = Guid.NewGuid(),
			Status = VeilingKlokStatus.Scheduled,
			PeakedLiveViews = 0,
			CreatedAt = DateTimeOffset.UtcNow,
			RegionOrState = "Region",
			Country = "NL",
			CurrentBids = 0,
			TotalProducts = 0,
			Products = new List<ProductOutputDto>()
		};
		mediator.RegisterResponse<GetVeilingKlokCommand, VeilingKlokOutputDto>(klok);

		var controller = CreateController(mediator);

		var result = await controller.GetVeilingKlok(Guid.NewGuid());

		var success = Assert.IsType<HttpSuccess<VeilingKlokOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
	}

	[Fact]
	public async Task GetProductDetails_WithValidId_ReturnsDetailsInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var details = new ProductDetailsOutputDto
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTimeOffset.UtcNow,
			Name = "Product",
			Description = "Desc",
			AuctionPrice = null,
			MinimumPrice = 1,
			Stock = 10,
			ImageBase64 = "img",
			Dimension = "10x10",
			Auctioned = false,
			AuctionedCount = 0,
			AuctionedAt = null,
			KwekerId = Guid.NewGuid(),
			CompanyName = "Company"
		};
		mediator.RegisterResponse<GetProductDetailsCommand, ProductDetailsOutputDto>(details);

		var controller = CreateController(mediator);

		var result = await controller.GetProductDetails(Guid.NewGuid());

		var success = Assert.IsType<HttpSuccess<ProductDetailsOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
	}

	[Fact]
	public async Task UpdateProductPrice_WithValidData_ReturnsUpdatedProductInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var details = new ProductDetailsOutputDto
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTimeOffset.UtcNow,
			Name = "Updated",
			Description = "Desc",
			AuctionPrice = null,
			MinimumPrice = 2,
			Stock = 5,
			ImageBase64 = "img",
			Dimension = "10x10",
			Auctioned = false,
			AuctionedCount = 0,
			AuctionedAt = null,
			KwekerId = Guid.NewGuid(),
			CompanyName = "Company"
		};
		mediator.RegisterResponse<UpdateProductCommand, ProductDetailsOutputDto>(details);

		var controller = CreateController(mediator);

		var dto = new UpdateProductDTO
		{
			MinimumPrice = 2
		};

		var result = await controller.UpdateProductPrice(Guid.NewGuid(), Guid.NewGuid(), dto);

		var success = Assert.IsType<HttpSuccess<ProductDetailsOutputDto>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
		Assert.Equal("Product price updated successfully", success.Message);
	}

	[Fact]
	public async Task GetProducts_WhenCalled_ReturnsPaginatedProductsInHttpSuccess()
	{
		var mediator = new FakeMediator();
		var products = new PaginatedOutputDto<ProductOutputDto>
		{
			Page = 1,
			Limit = 10,
			TotalCount = 0,
			Data = new List<ProductOutputDto>()
		};
		mediator.RegisterResponse<GetProductsQuery, PaginatedOutputDto<ProductOutputDto>>(products);

		var controller = CreateController(mediator);

		var result = await controller.GetProducts(null, null, null, 1, 10);

		var success = Assert.IsType<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(result);
		Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
	}

	[Fact]
	public async Task StartVeilingProduct_WithValidIds_ReturnsNoContentHttpSuccess()
	{
		var mediator = new FakeMediator();
		var controller = CreateController(mediator);

		var result = await controller.StartVeilingProduct(Guid.NewGuid(), Guid.NewGuid());

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status204NoContent, success.StatusCode);
		Assert.Equal("Veiling product started successfully", success.Message);
	}

	[Fact]
	public async Task UpdateVeilingKlokStatus_WithValidStatus_ReturnsNoContentHttpSuccess()
	{
		var mediator = new FakeMediator();
		var controller = CreateController(mediator);

		var result = await controller.UpdateVeilingKlokStatus(Guid.NewGuid(), VeilingKlokStatus.Started);

		var success = Assert.IsType<HttpSuccess<object>>(result);
		Assert.Equal(StatusCodes.Status204NoContent, success.StatusCode);
		Assert.Equal("VeilingKlok status updated successfully", success.Message);
	}
}
