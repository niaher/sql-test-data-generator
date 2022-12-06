namespace TestDataGenerator.Domain;

public class Shipment
{
    public int Id { get; init; }
    public required string CaseNumber { get; init; }
    public string Status { get; private set; } = "Unknown";
    public int FreightRateId { get; init; }

    public void ApplyTrackingData(TrackingData trackingData)
    {
        Status = trackingData.EstimateTimeDeparture <= DateTime.Today.AddDays(-120)
            ? "Completed"
            : trackingData.Arrival <= DateTime.Today
                ? "Completed"
                : "Pending";
    }
}