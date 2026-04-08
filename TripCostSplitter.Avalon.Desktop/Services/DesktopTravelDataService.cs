using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Desktop.Services;

public class DesktopTravelDataService : JsonTravelDataService
{
    private const string FileName = "travel_data.json";

    public DesktopTravelDataService() 
        : base(Path.Combine(AppContext.BaseDirectory, FileName))
    {
    }
}
