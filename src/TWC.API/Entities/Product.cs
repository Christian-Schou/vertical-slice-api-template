﻿using System.Text.Json.Serialization;

namespace TWC.API.Entities;

public sealed class Product : Entity
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0.0m;

    public List<string> Categories { get; set; } = [];

    public DateTimeOffset LastUpdatedOnUtc { get; set; }

    public static Product Create(string name, string description, decimal price, List<string> categories)
    {
        var product = new Product(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            Price = price,
            Categories = categories,
            LastUpdatedOnUtc = DateTimeOffset.UtcNow
        };

        return product;
    }

    private Product(Guid id) : base(id) { }
    
    // For EF Core
    [JsonConstructor]
    public Product() { }
}

public sealed record ProductCreatedEvent(Guid Id);
