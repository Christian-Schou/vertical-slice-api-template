namespace TWC.API.Features.Products.UpdateProduct;

public sealed record UpdateProductRequest(
    Guid Id,
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/products", async (UpdateProductRequest request, IMessageBus bus) =>
            {
                var command = new UpdateProductCommand(
                    request.Id,
                    request.Name,
                    request.Description,
                    request.Categories,
                    request.Price);

                var result = await bus.InvokeAsync<Result<UpdateProductResult>>(command);

                return result.Match(
                    () => Results.Ok(result.Value.IsSuccess),
                    error => Results.BadRequest(error));
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product")
            .WithDescription("Update Product");
    }
}