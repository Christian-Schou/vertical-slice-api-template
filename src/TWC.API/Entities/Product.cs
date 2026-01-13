namespace TWC.API.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0.0m;

    public List<string> Categories { get; set; } = [];

    public DateTimeOffset LastUpdatedOnUtc { get; set; }
}