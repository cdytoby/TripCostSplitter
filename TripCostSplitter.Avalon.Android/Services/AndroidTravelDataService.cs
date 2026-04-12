using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Android.Services;

public class AndroidTravelDataService()
    : JsonTravelDataService(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
