using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Android.Services;

public class AndroidTravelDataService : JsonTravelDataService
{
    private const string FileName = "travel_data.json";

    public AndroidTravelDataService() 
        : base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FileName))
    {
    }
}
