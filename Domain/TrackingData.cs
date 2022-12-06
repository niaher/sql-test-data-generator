namespace TestDataGenerator.Domain;

public class TrackingData
{
    public int ShipmentId { get; init; }
    public DateTime? EstimateTimeDeparture { get; set; }
    public DateTime? EstimateTimeArrival { get; set; }
    public DateTime? ActualTimeDeparture { get; set; }
    public DateTime? ActualTimeArrival { get; set; }

    public DateTime? Arrival => ActualTimeArrival ?? EstimateTimeArrival;
    public DateTime? Departure => ActualTimeDeparture ?? EstimateTimeDeparture;
}