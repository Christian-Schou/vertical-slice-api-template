namespace VSATemplate.Features.Products.GetProducts;

public sealed class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/products", async (IMessageBus bus) =>
            {
                var result = await bus.InvokeAsync<Result<IEnumerable<GetProductsResult>>>(new GetProductsQuery());

                return result.Match(
                    () => Results.Ok(result.Value),
                    error => Results.BadRequest(error));
            })
            .WithName("GetProducts")
            .Produces<IEnumerable<GetProductsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
    }
}