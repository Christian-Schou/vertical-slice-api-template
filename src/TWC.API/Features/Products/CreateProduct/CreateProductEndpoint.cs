namespace TWC.API.Features.Products.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed record CreateProductResponse(Guid Id);

public sealed class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/products", async (CreateProductRequest request, IMessageBus bus) =>
            {
                var command = new CreateProductCommand(
                    request.Name,
                    request.Description,
                    request.Categories,
                    request.Price);

                var result = await bus.InvokeAsync<Result<CreateProductResult>>(command);

                return result.Match(
                    () => Results.Created($"api/products/{result.Value.Id}", result.Value),
                    error => Results.BadRequest(error));
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
    }
}