namespace VSATemplate.Features.Products.GetProductById;

public sealed class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/products/{id}", async (Guid id, IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<GetProductByIdResult>>(new GetProductByIdQuery(id));

                return result.Match(
                    () => Results.Ok(result.Value),
                    error => Results.BadRequest(error));
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product by Id")
            .WithDescription("Get Product by Id");
    }
}