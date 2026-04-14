using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Desktop.Services;

public class DesktopDataService(): JsonDataService(Path.Combine(AppContext.BaseDirectory, "AppData"));
