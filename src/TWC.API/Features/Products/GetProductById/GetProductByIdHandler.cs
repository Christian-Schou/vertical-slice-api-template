﻿using Marten;

namespace TWC.API.Features.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid Id);

public sealed record GetProductByIdResult(
    Guid Id,
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed class GetProductByIdQueryHandler(IQuerySession session)
{
    public async Task<Result<GetProductByIdResult>> Handle(GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

        if (product is null) return Result.Fail<GetProductByIdResult>(ProductErrors.NotFound(query.Id));

        var result = new GetProductByIdResult(
            product.Id,
            product.Name,
            product.Description,
            product.Categories,
            product.Price);

        return result;
    }
}