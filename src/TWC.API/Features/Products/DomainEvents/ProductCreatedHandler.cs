namespace TWC.API.Features.Products.DomainEvents;

public sealed class ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
{
    public void Handle(ProductCreatedEvent notification)
    {
        logger.LogInformation("Domain Event: Product created with Id {ProductId}", notification.Id);
    }
}

