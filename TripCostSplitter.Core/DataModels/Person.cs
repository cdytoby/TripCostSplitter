using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class Person(string id, string name): ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = id;
    
    [ObservableProperty]
    public partial string Name { get; set; } = name;
}