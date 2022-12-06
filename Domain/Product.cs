namespace TestDataGenerator.Domain;

public class Product
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int QuantityPerContainer { get; init; }
}