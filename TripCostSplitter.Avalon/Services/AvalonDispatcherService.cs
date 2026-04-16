using Avalonia.Threading;
using TripCostSplitter.AppBase.Services;

namespace TripCostSplitter.Avalon.Services;

public class AvalonDispatcherService: IAppDispatcherService
{
    public void Invoke(Action callback)
    {
        Dispatcher.UIThread.Invoke(callback);
    }
}