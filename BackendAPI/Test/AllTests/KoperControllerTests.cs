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

public class KoperControllerTests
{
    private static KoperController CreateController(FakeMediator mediator)
    {
        var controller = new KoperController(mediator, new FakeSignInManager())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        return controller;
    }

    private static void SetUser(KoperController controller, Guid accountId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, accountId.ToString()),
            new(ClaimTypes.Role, AccountType.Koper.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task CreateAccount_WithValidData_ReturnsCreatedAuthOutputInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var koperId = Guid.NewGuid();
        var koper = new global::Domain.Entities.Koper("koper@example.com")
        {
            Id = koperId,
            FirstName = "First",
            LastName = "Last",
            Telephone = "1234567890",
        };
        mediator.RegisterResponse<CreateKoperCommand, global::Domain.Entities.Koper>(koper);

        var controller = CreateController(mediator);

        var dto = new CreateKoperDTO
        {
            Email = "koper@example.com",
            Password = "password",
            FirstName = "First",
            LastName = "Last",
            Telephone = "1234567890",
            Address = new AddressInputDto
            {
                Street = "Street 1",
                City = "City",
                RegionOrState = "Region",
                PostalCode = "1234AB",
                Country = "NL",
            },
        };

        var result = await controller.CreateAccount(dto);

        var success = Assert.IsType<HttpSuccess<Guid>>(result);
        Assert.Equal(StatusCodes.Status201Created, success.StatusCode);
        Assert.Equal("Koper account created and authenticated successfully", success.Message);
        Assert.Equal(koperId, success.Data);
    }

    [Fact]
    public async Task UpdateAccount_WithKoperClaims_ReturnsUpdatedAccountInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var accountResult = new AccountOutputDto
        {
            AccountType = AccountType.Koper,
            Email = "koper@example.com",
        };
        mediator.RegisterResponse<UpdateKoperCommand, AccountOutputDto>(accountResult);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var dto = new UpdateKoperDTO
        {
            Email = "koper@example.com",
            Password = "password",
            FirstName = "First",
            LastName = "Last",
            Telephone = "1234567890",
            Address = new AddressInputDto
            {
                Street = "Street 1",
                City = "City",
                RegionOrState = "Region",
                PostalCode = "1234AB",
                Country = "NL",
            },
        };

        var result = await controller.UpdateAccount(dto);

        var success = Assert.IsType<HttpSuccess<AccountOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Koper account updated successfully", success.Message);
    }

    [Fact]
    public async Task CreateAddress_WithKoperClaims_ReturnsCreatedAddressInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var addressResult = new AddressOutputDto
        {
            Id = 1,
            Street = "Street 1",
            City = "City",
            RegionOrState = "Region",
            PostalCode = "1234AB",
            Country = "NL",
        };
        mediator.RegisterResponse<CreateAddressCommand, AddressOutputDto>(addressResult);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var dto = new AddressInputDto
        {
            Street = "Street 1",
            City = "City",
            RegionOrState = "Region",
            PostalCode = "1234AB",
            Country = "NL",
        };

        var result = await controller.CreateAddress(dto);

        var success = Assert.IsType<HttpSuccess<AddressOutputDto>>(result);
        Assert.Equal(StatusCodes.Status201Created, success.StatusCode);
        Assert.Equal("Address created successfully", success.Message);
    }

    [Fact]
    public async Task UpdatePrimaryAddress_WithKoperClaims_ReturnsUpdatedAddressInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var addressResult = new AddressOutputDto
        {
            Id = 1,
            Street = "Street 1",
            City = "City",
            RegionOrState = "Region",
            PostalCode = "1234AB",
            Country = "NL",
        };
        mediator.RegisterResponse<UpdatePrimaryAddressCommand, AddressOutputDto>(addressResult);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var result = await controller.UpdatePrimaryAddress(1);

        var success = Assert.IsType<HttpSuccess<AddressOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Primary address updated successfully", success.Message);
    }

    [Fact]
    public async Task CreateOrder_WithKoperClaims_ReturnsCreatedOrderInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var orderResult = new OrderOutputDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Status = OrderStatus.Open,
            TotalAmount = 10,
            TotalItems = 1,
        };
        mediator.RegisterResponse<CreateOrderCommand, OrderOutputDto>(orderResult);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var dto = new CreateOrderDTO { VeilingKlokId = Guid.NewGuid() };

        var result = await controller.CreateOrder(dto);

        var success = Assert.IsType<HttpSuccess<OrderOutputDto>>(result);
        Assert.Equal(StatusCodes.Status201Created, success.StatusCode);
        Assert.Equal("Order created successfully", success.Message);
    }

    [Fact]
    public async Task GetOrder_WithKoperClaims_ReturnsOrderDetailsInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var details = new OrderDetailsOutputDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Status = OrderStatus.Open,
            TotalAmount = 20,
            TotalItems = 2,
            Products = new List<OrderItemOutputDto>(),
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
    public async Task GetOrders_WithKoperClaims_ReturnsPaginatedOrdersInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var orders = new PaginatedOutputDto<OrderOutputDto>
        {
            Page = 1,
            Limit = 10,
            TotalCount = 0,
            Data = new List<OrderOutputDto>(),
        };
        mediator.RegisterResponse<GetOrdersCommand, PaginatedOutputDto<OrderOutputDto>>(orders);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var result = await controller.GetOrders(null, null, null, 1, 10);

        var success = Assert.IsType<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
    }

    [Fact]
    public async Task OrderProduct_WithKoperClaims_ReturnsCreatedOrderItemInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var item = new OrderItemOutputDto
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            ProductDescription = "Desc",
            ProductImageUrl = "img",
            CompanyName = "Company",
            Quantity = 1,
            PriceAtPurchase = 10,
            OrderedAt = DateTimeOffset.UtcNow,
        };
        mediator.RegisterResponse<CreateOrderProductCommand, OrderItemOutputDto>(item);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var result = await controller.OrderProduct(Guid.NewGuid(), Guid.NewGuid(), 1);

        var success = Assert.IsType<HttpSuccess<OrderItemOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Product ordered successfully", success.Message);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProductInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var product = new ProductOutputDto
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            Description = "Desc",
            ImageUrl = "img",
            AuctionedPrice = null,
            MinimumPrice = null,
            AuctionedAt = null,
            Region = null,
            Dimension = "10x10",
            Stock = 10,
            CompanyName = "Company",
            AuctionPlanned = false,
        };
        mediator.RegisterResponse<GetProductCommand, ProductOutputDto>(product);

        var controller = CreateController(mediator);

        var result = await controller.GetProduct(Guid.NewGuid());

        var success = Assert.IsType<HttpSuccess<ProductOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
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
            Data = new List<ProductOutputDto>(),
        };
        mediator.RegisterResponse<GetProductsQuery, PaginatedOutputDto<ProductOutputDto>>(products);

        var controller = CreateController(mediator);

        var result = await controller.GetProducts(null, null, null, null, 1, 10);

        var success = Assert.IsType<HttpSuccess<PaginatedOutputDto<ProductOutputDto>>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
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
            VeilingDurationSeconds = 1800,
            Products = new List<ProductOutputDto>(),
        };
        mediator.RegisterResponse<GetVeilingKlokCommand, VeilingKlokOutputDto>(klok);

        var controller = CreateController(mediator);

        var result = await controller.GetVeilingKlok(Guid.NewGuid());

        var success = Assert.IsType<HttpSuccess<VeilingKlokOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
    }
}
