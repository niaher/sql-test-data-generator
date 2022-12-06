namespace TestDataGenerator.Domain;

public class FreightRate
{
    public int Id { get; init; }
    public required string Origin { get; init; }
    public required string Destination { get; init; }
    public decimal ContainerPrice { get; init; }
    public int FreightForwarderId { get; init; }
}