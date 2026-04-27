using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase;
using TripCostSplitter.Core.Services;
using TripCostSplitter.NTest.Services;

namespace TripCostSplitter.NTest;

public static class TestSetup
{
    public static IServiceProvider ServiceProvider { get; } = GetProvider();
    
    private static IServiceProvider GetProvider()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAppServices();
        serviceCollection.AddSingleton<IDataService, MockDataService>();
        return serviceCollection.BuildServiceProvider();
    }
}