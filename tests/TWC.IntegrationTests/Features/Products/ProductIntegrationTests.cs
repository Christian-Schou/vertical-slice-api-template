using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TWC.API.Entities;
using TWC.API.Features.Products.CreateProduct;
using TWC.API.Features.Products.GetProductById;
using TWC.API.Features.Products.GetProducts;
using TWC.API.Features.Products.UpdateProduct;
using TWC.IntegrationTests.Common;

namespace TWC.IntegrationTests.Features.Products;

public class ProductIntegrationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request =
            new CreateProductCommand("Test Product", "Test Description", new List<string> { "Category1" }, 10.99m);

        // Act
        var response = await _client.PostAsJsonAsync("api/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var productResponse = await response.Content.ReadFromJsonAsync<CreateProductResult>();
        productResponse.Should().NotBeNull();
        productResponse!.Id.Should().NotBeEmpty();

        var dbProduct = await Session.LoadAsync<Product>(productResponse.Id);
        dbProduct.Should().NotBeNull();
        dbProduct!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = Product.Create(
            "Existing Product",
            "Description",
            100,
            new List<string> { "Cat" }
        );
        Session.Store(product);
        await Session.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"api/products/{product.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedProduct = await response.Content.ReadFromJsonAsync<GetProductByIdResult>();
        returnedProduct.Should().NotBeNull();
        returnedProduct!.Id.Should().Be(product.Id);
        returnedProduct.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Act
        // Note: The endpoint returns 400 BadRequest for not found based on current implementation 
        // (usually 404 is better, but following existing code logic from earlier readings)
        var response = await _client.GetAsync($"api/products/{Guid.NewGuid()}");

        // Assert
        // Checking implementation again... 
        // Logic was: if (product is null) return Result.Fail(Errors.NotFound) -> Results.BadRequest(error)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnList_WhenProductsExist()
    {
        // Arrange
        Session.Store(Product.Create("P1", "D1", 10, new List<string>()));
        Session.Store(Product.Create("P2", "D2", 20, new List<string>()));
        await Session.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<GetProductsResult>>();
        
        products.Should().NotBeNull();
        products!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = Product.Create("Old Name", "Old Desc", 50, new List<string>());
        Session.Store(product);
        await Session.SaveChangesAsync();

        var updateRequest =
            new UpdateProductCommand(product.Id, "New Name", "New Desc", new List<string> { "New Cat" }, 60);

        // Act
        var response = await _client.PutAsJsonAsync("api/products", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var querySession = Store.QuerySession();
        var refreshedProduct = await querySession.LoadAsync<Product>(product.Id);
        refreshedProduct!.Name.Should().Be("New Name");
        refreshedProduct.Price.Should().Be(60);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = Product.Create("To Delete", "Desc", 10, new List<string>());
        Session.Store(product);
        await Session.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var querySession = Store.QuerySession();
        var deletedProduct = await querySession.LoadAsync<Product>(product.Id);
        deletedProduct.Should().BeNull();
    }
}