namespace TripCostSplitter.AppBase.Services;

public interface IAppDispatcherService
{
    void Invoke(Action callback);
}