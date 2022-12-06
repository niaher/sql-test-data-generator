// See https://aka.ms/new-console-template for more information

using Bogus;
using Humanizer;
using TestDataGenerator;
using TestDataGenerator.Domain;

var faker = new Faker();

var freightForwarders = Enumerable.Range(1, 5).Select(t => new FreightForwarder
{
    Id = t,
    Name = faker.Company.CompanyName(),
}).ToList();

var products = Enumerable.Range(1, 100).Select(t => new Product
{
    Id = t,
    Name = $"{faker.Random.AlphaNumeric(4).ToUpper()} - " +
           $"{faker.PickRandom<ConsoleColor>().Humanize(LetterCasing.Title)} " +
           $"{faker.Commerce.Product()}",
    QuantityPerContainer = faker.Random.Number(1, 10) * 1000
}).ToList();

var freightRates = Enumerable.Range(1, 50).Select(t =>
{
    var origin = new Faker().Address;
    var destination = new Faker().Address;

    return new FreightRate
    {
        Id = t,
        Origin = $"{origin.City()}, {origin.Country()}",
        Destination = $"{destination.City()}, {destination.Country()}",
        ContainerPrice = faker.Random.Decimal(2000, 10000),
        FreightForwarderId = faker.PickRandom(freightForwarders).Id
    };
}).ToList();

var shipments = Enumerable.Range(1, 100).Select(t => new Shipment
{
    Id = t,
    CaseNumber = $"C{t:000}",
    FreightRateId = faker.PickRandom(freightRates).Id
}).ToList();

var shipmentItems = shipments
    .SelectMany(t =>
    {
        return Enumerable.Range(1, 3)
            .Select(x =>
            {
                var product = faker.PickRandom(products);

                return new ShippedItem
                {
                    Id = t.Id * 10 + x,
                    ShipmentId = t.Id,
                    ProductId = product.Id,
                    Quantity = product.QuantityPerContainer * faker.Random.Number(1, 3)
                };
            });
    })
    .ToList();

var trackAndTrace = shipments
    .Select(t =>
    {
        var etd = new DateTime(2021, 1, 1).AddDays(t.Id * 8);
        var estimatedTransitTime = faker.Random.Number(10, 80);
        var slowdown = faker.Random.Number(80, 120) / 100;

        var etdIsNull = faker.Random.Bool();
        var atdIsNull = !etdIsNull && faker.Random.Bool();
        var ataIsNull = !atdIsNull && faker.Random.Bool();

        return new TrackingData
        {
            ShipmentId = t.Id,
            EstimateTimeDeparture = etdIsNull ? null : etd,
            EstimateTimeArrival = etdIsNull ? null : etd.AddDays(estimatedTransitTime),
            ActualTimeDeparture = atdIsNull ? null : etd.AddDays(faker.Random.Number(0, 30)),
            ActualTimeArrival = ataIsNull ? null : etd.AddDays(estimatedTransitTime * slowdown)
        };
    })
    .ToList();

foreach (var shipment in shipments)
{
    var trackingData = trackAndTrace.Single(t => t.ShipmentId == shipment.Id);
    shipment.ApplyTrackingData(trackingData);
}

var sb = new SqlBuilder();

sb.BuildSql(
    freightForwarders,
    t => t.Id,
    t => t.Name);

sb.BuildSql(
    products,
    t => t.Id,
    t => t.Name,
    t => t.QuantityPerContainer);

sb.BuildSql(
    shipments,
    t => t.Id,
    t => t.CaseNumber,
    t => t.FreightRateId,
    t => t.Status);

sb.BuildSql(
    shipmentItems,
    t => t.Id,
    t => t.ShipmentId,
    t => t.ProductId,
    t => t.Quantity);

sb.BuildSql(
    freightRates,
    t => t.Id,
    t => t.FreightForwarderId,
    t => t.Origin,
    t => t.Destination,
    t => t.ContainerPrice);

sb.BuildSql(trackAndTrace,
    t => t.ShipmentId,
    t => t.EstimateTimeDeparture,
    t => t.EstimateTimeArrival,
    t => t.ActualTimeDeparture,
    t => t.ActualTimeArrival);

Console.Write(sb.Sql);
Console.ReadLine();