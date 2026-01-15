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

public class KwekerControllerTests
{
    private static KwekerController CreateController(FakeMediator mediator)
    {
        var controller = new KwekerController(mediator, new FakeSignInManager())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        return controller;
    }

    private static void SetUser(KwekerController controller, Guid accountId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, accountId.ToString()),
            new(ClaimTypes.Role, AccountType.Kweker.ToString()),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task CreateAccount_WithValidData_ReturnsAuthOutputInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var kwekerId = Guid.NewGuid();
        var kweker = new global::Domain.Entities.Kweker("kweker@example.com")
        {
            Id = kwekerId,
            KvkNumber = "KVK123",
            CompanyName = "Company",
            FirstName = "First",
            LastName = "Last",
            Telephone = "1234567890",
        };
        mediator.RegisterResponse<CreateKwekerCommand, global::Domain.Entities.Kweker>(kweker);

        var controller = CreateController(mediator);

        var dto = new CreateKwekerDTO
        {
            CompanyName = "Company",
            FirstName = "First",
            LastName = "Last",
            Email = "kweker@example.com",
            Password = "password",
            Telephone = "1234567890",
            KvkNumber = "KVK123",
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
        Assert.Equal("Kweker account created and authenticated successfully", success.Message);
        Assert.Equal(kwekerId, success.Data);
    }

    [Fact]
    public async Task UpdateAccount_WithKwekerClaims_ReturnsUpdatedAccountInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var accountResult = new AccountOutputDto
        {
            AccountType = AccountType.Kweker,
            Email = "kweker@example.com",
        };
        mediator.RegisterResponse<UpdateKwekerCommand, AccountOutputDto>(accountResult);

        var controller = CreateController(mediator);
        var accountId = Guid.NewGuid();
        SetUser(controller, accountId);

        var dto = new UpdateKwekerDTO { CompanyName = "New Company" };

        var result = await controller.UpdateAccount(dto);

        var success = Assert.IsType<HttpSuccess<AccountOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Kweker account updated successfully", success.Message);

        var sentCommand = Assert.IsType<UpdateKwekerCommand>(mediator.LastRequest);
        Assert.Equal(accountId, sentCommand.AccountId);
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
            TotalItems = 1,
        };
        mediator.RegisterResponse<UpdateOrderProductCommand, OrderOutputDto>(orderResult);

        var controller = CreateController(mediator);

        var result = await controller.UpdateOrderProduct(Guid.NewGuid(), Guid.NewGuid(), 5);

        var success = Assert.IsType<HttpSuccess<OrderOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Order product updated successfully", success.Message);
    }

    [Fact]
    public async Task GetOrder_WithValidOrderId_ReturnsOrderDetailsInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var kwekerId = Guid.NewGuid();
        var details = new OrderKwekerOutput
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Status = OrderStatus.Open,
            ClosedAt = null,
            Quantity = 2,
            TotalPrice = 20.0m,
            Product = new ProductOutputDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test",
                ImageUrl = "test.jpg",
                AuctionedPrice = 15.0m,
                MinimumPrice = 10.0m,
                AuctionedAt = null,
                Region = null,
                Dimension = "S",
                Stock = 10,
                CompanyName = "Test Company",
                AuctionPlanned = false,
            },
            KoperInfo = new KoperInfoOutputDto
            {
                FirstName = "Test",
                LastName = "Koper",
                Email = "test@example.com",
                Telephone = "123456789",
                Address = new AddressOutputDto
                {
                    Id = 1,
                    Street = "Test St",
                    City = "Test City",
                    RegionOrState = "Test Region",
                    PostalCode = "12345",
                    Country = "NL",
                },
            },
        };
        mediator.RegisterResponse<GetKwekerOrderCommand, OrderKwekerOutput>(details);

        var controller = CreateController(mediator);
        SetUser(controller, kwekerId);

        var result = await controller.GetOrder(Guid.NewGuid());

        var success = Assert.IsType<HttpSuccess<OrderKwekerOutput>>(result);
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
    public async Task CreateProduct_WithKwekerClaims_ReturnsCreatedProductDetailsInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var productDetails = new ProductDetailsOutputDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "Product",
            Description = "Desc",
            AuctionPrice = null,
            MinimumPrice = 1,
            Stock = 10,
            Region = null,
            ImageBase64 = "img",
            Dimension = "10x10",
            Auctioned = false,
            AuctionedCount = 0,
            AuctionedAt = null,
            KwekerId = Guid.NewGuid(),
            CompanyName = "Company",
        };
        mediator.RegisterResponse<CreateProductCommand, ProductDetailsOutputDto>(productDetails);

        var controller = CreateController(mediator);
        var kwekerId = Guid.NewGuid();
        SetUser(controller, kwekerId);

        var dto = new CreateProductDTO
        {
            Name = "Product",
            Description = "Desc",
            MinimumPrice = 1,
            Stock = 10,
            ImageBase64 = "img",
            Dimension = "10x10",
        };

        var result = await controller.CreateProduct(dto);

        var success = Assert.IsType<HttpSuccess<ProductDetailsOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Product created successfully", success.Message);
    }

    [Fact]
    public async Task GetProductDetails_WithValidProductId_ReturnsDetailsInHttpSuccess()
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
            Region = null,
            ImageBase64 = "img",
            Dimension = "10x10",
            Auctioned = false,
            AuctionedCount = 0,
            AuctionedAt = null,
            KwekerId = Guid.NewGuid(),
            CompanyName = "Company",
        };
        mediator.RegisterResponse<GetProductDetailsCommand, ProductDetailsOutputDto>(details);

        var controller = CreateController(mediator);

        var result = await controller.GetProductDetails(Guid.NewGuid());

        var success = Assert.IsType<HttpSuccess<ProductDetailsOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
    }

    [Fact]
    public async Task GetProductOrders_WithKwekerClaims_ReturnsOrdersInHttpSuccess()
    {
        var mediator = new FakeMediator();
        var orders = new PaginatedOutputDto<OrderOutputDto>
        {
            Page = 1,
            Limit = 10,
            TotalCount = 1,
            Data = new List<OrderOutputDto>(),
        };
        mediator.RegisterResponse<GetProductOrdersCommand, PaginatedOutputDto<OrderOutputDto>>(
            orders
        );

        var controller = CreateController(mediator);
        var kwekerId = Guid.NewGuid();
        SetUser(controller, kwekerId);

        var result = await controller.GetProductOrders(
            Guid.NewGuid(),
            OrderStatus.Open,
            DateTime.UtcNow,
            null,
            1,
            10
        );

        var success = Assert.IsType<HttpSuccess<PaginatedOutputDto<OrderOutputDto>>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithKwekerClaims_ReturnsUpdatedProductDetailsInHttpSuccess()
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
            Region = null,
            ImageBase64 = "img",
            Dimension = "10x10",
            Auctioned = false,
            AuctionedCount = 0,
            AuctionedAt = null,
            KwekerId = Guid.NewGuid(),
            CompanyName = "Company",
        };
        mediator.RegisterResponse<UpdateProductCommand, ProductDetailsOutputDto>(details);

        var controller = CreateController(mediator);
        var kwekerId = Guid.NewGuid();
        SetUser(controller, kwekerId);

        var dto = new UpdateProductDTO { Name = "Updated" };

        var result = await controller.UpdateProduct(Guid.NewGuid(), dto);

        var success = Assert.IsType<HttpSuccess<ProductDetailsOutputDto>>(result);
        Assert.Equal(StatusCodes.Status200OK, success.StatusCode);
        Assert.Equal("Product updated successfully", success.Message);
    }

    [Fact]
    public async Task GetProducts_WithKwekerClaims_ReturnsPaginatedProductsInHttpSuccess()
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
        var kwekerId = Guid.NewGuid();
        SetUser(controller, kwekerId);

        var result = await controller.GetProducts(null, null, 1, 10);

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
