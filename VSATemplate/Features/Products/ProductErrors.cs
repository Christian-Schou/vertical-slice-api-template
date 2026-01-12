namespace VSATemplate.Features.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid id)
    {
        return new Error($"The product with Id '{id}' was not found")
            .WithMetadata("Code", "Products.NotFound");
    }
}