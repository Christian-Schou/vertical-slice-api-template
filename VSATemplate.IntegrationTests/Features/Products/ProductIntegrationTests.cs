using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VSATemplate.Entities;
using VSATemplate.Features.Products.CreateProduct;
using VSATemplate.Features.Products.GetProductById;
using VSATemplate.Features.Products.GetProducts;
using VSATemplate.Features.Products.UpdateProduct;
using VSATemplate.IntegrationTests.Common;

namespace VSATemplate.IntegrationTests.Features.Products;

public class ProductIntegrationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request =
            new CreateProductRequest("Test Product", "Test Description", new List<string> { "Category1" }, 10.99m);

        // Act
        var response = await _client.PostAsJsonAsync("api/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var productResponse = await response.Content.ReadFromJsonAsync<CreateProductResult>();
        productResponse.Should().NotBeNull();
        productResponse!.Id.Should().NotBeEmpty();

        var dbProduct = await DbContext.Products.FindAsync(productResponse.Id);
        dbProduct.Should().NotBeNull();
        dbProduct!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Description = "Description",
            Categories = new List<string> { "Cat" },
            Price = 100
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

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
        DbContext.Products.Add(new Product
            { Id = Guid.NewGuid(), Name = "P1", Description = "D1", Categories = new List<string>(), Price = 10 });
        DbContext.Products.Add(new Product
            { Id = Guid.NewGuid(), Name = "P2", Description = "D2", Categories = new List<string>(), Price = 20 });
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<GetProductsResult>>();
        products.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Desc",
            Categories = new List<string>(),
            Price = 50
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        var updateRequest =
            new UpdateProductRequest(product.Id, "New Name", "New Desc", new List<string> { "New Cat" }, 60);

        // Act
        var response = await _client.PutAsJsonAsync("api/products", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var refreshedProduct = await DbContext.Products.FindAsync(product.Id);
        refreshedProduct!.Name.Should().Be("New Name");
        refreshedProduct.Price.Should().Be(60);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(), Name = "To Delete", Description = "Desc", Categories = new List<string>(), Price = 10
        };
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var deletedProduct = await DbContext.Products.FindAsync(product.Id);
        deletedProduct.Should().BeNull();
    }
}