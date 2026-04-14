using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Android.Services;

public class AndroidDataService()
    : JsonDataService(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
