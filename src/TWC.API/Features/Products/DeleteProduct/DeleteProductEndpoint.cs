namespace TWC.API.Features.Products.DeleteProduct;

public sealed class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/products/{id}", async (Guid id, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<DeleteProductResult>>(new DeleteProductCommand(id));

                result.Match(
                    () => Results.Ok(result.Value.IsSuccess),
                    error => Results.BadRequest(error));
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
    }
}