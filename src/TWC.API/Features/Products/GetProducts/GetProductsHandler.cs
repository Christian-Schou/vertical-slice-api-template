﻿using Marten;

namespace TWC.API.Features.Products.GetProducts;

public sealed record GetProductsQuery;

public sealed record GetProductsResult(
    Guid Id,
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed class GetProductsQueryHandler(IQuerySession session)
{
    public async Task<Result<IEnumerable<GetProductsResult>>> Handle(GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var products = await session
            .Query<Product>()
            .ToListAsync(cancellationToken);

        return products.Select(item =>
            new GetProductsResult(
                item.Id,
                item.Name,
                item.Description,
                item.Categories,
                item.Price)).ToList();
    }
}