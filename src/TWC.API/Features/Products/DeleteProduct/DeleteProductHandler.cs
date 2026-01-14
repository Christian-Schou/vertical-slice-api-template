﻿using Marten;

namespace TWC.API.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id);

public sealed record DeleteProductResult(bool IsSuccess);

public sealed class DeleteProductCommandHandler(IDocumentSession session)
{
    public async Task<Result<DeleteProductResult>> Handle(DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
        
        if (product is null) return Result.Fail<DeleteProductResult>(ProductErrors.NotFound(command.Id));

        session.Delete(product);

        return new DeleteProductResult(true);
    }
}