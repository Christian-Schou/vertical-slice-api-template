namespace VSATemplate.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id);

public sealed record DeleteProductResult(bool IsSuccess);

public sealed class DeleteProductCommandHandler(ApplicationDbContext dbContext)
{
    public async Task<Result<DeleteProductResult>> Handle(DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await dbContext
            .Products
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (product is null) return Result.Fail<DeleteProductResult>(ProductErrors.NotFound(command.Id));

        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}