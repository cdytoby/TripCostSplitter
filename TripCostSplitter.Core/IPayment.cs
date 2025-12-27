namespace TripCostSplitter.Core;

public interface IPayment
{
    DateTime Date { get; set; }
    string? Description { get; set; }
}