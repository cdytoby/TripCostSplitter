using System.Threading.Tasks;

namespace TripCostSplitter.Avalon.Services;

public interface INavigationService
{
    Task PushAsync<TViewModel>(params object[] parameters) where TViewModel : class;
    Task PopAsync();
}
