using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace TripCostSplitter.Avalon.Android;

[Activity(
    Label = "TripCostSplitter.Avalon.Android",
    Theme = "@style/MyTheme.NoActionBar",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
