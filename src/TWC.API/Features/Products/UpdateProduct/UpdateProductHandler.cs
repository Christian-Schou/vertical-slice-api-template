﻿﻿using Marten;

namespace TWC.API.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed record UpdateProductResult(bool IsSuccess);

public sealed class UpdateProductCommandValidator :
    AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required!");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required!");
        RuleFor(x => x.Price)
            .GreaterThan(0.0m)
            .WithMessage("Price should be greater than zero!");
        RuleFor(x => x.Categories)
            .Must(x => x == null || x.Any())
            .WithMessage("Categories should have atleast one category!");
    }
}

public sealed class UpdateProductCommandHandler(IDocumentSession session)
{
    public async Task<Result<UpdateProductResult>> Handle(UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null) return Result.Fail<UpdateProductResult>(ProductErrors.NotFound(command.Id));

        product.Name = command.Name;
        product.Description = command.Description;
        product.Categories = command.Categories;
        product.Price = command.Price;
        product.LastUpdatedOnUtc = DateTime.UtcNow;

        session.Store(product);

        return new UpdateProductResult(true);
    }
}