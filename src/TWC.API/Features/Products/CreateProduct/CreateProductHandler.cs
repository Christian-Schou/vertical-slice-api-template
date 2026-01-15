﻿﻿﻿using Marten;

namespace TWC.API.Features.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    List<string> Categories,
    decimal Price);

public sealed record CreateProductResult(Guid Id);

public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
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
            .WithMessage("Categories should have at least one category!");
    }
}

public sealed class CreateProductCommandHandler(IDocumentSession session)
{
    public (Result<CreateProductResult>, ProductCreatedEvent) Handle(CreateProductCommand command)
    {
        var product = Product.Create(
            command.Name,
            command.Description,
            command.Price,
            command.Categories
        );

        session.Store(product);

        return (new CreateProductResult(product.Id), new ProductCreatedEvent(product.Id));
    }
}