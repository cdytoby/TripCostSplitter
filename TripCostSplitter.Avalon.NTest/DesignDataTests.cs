using TripCostSplitter.Avalon.DesignViewModels;

namespace TripCostSplitter.Avalon.NTest;

public class DesignDataTests
{
    [Test]
    public void TestIsWorking()
    {
        Assert.That(!string.IsNullOrEmpty(DesignData.TravelListViewModelDesign.Travels.First().TravelId));
        Assert.That(!string.IsNullOrEmpty(DesignData.ExampleSettingsViewModel.DefaultCurrency?.Name));
    }
}