namespace TripCostSplitter.AppBase.Services;

public interface INavigationService
{
    Task PushAsync(string pageId);
    Task PopAsync();
}
