using System;
using System.IO;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Desktop.Services;

public class DesktopTravelDataService(): JsonTravelDataService(AppContext.BaseDirectory);
