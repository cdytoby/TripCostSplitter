using System.Text.Json;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.Services;

public abstract class JsonTravelDataService : ITravelDataService
{
    protected readonly string _filePath;

    protected JsonTravelDataService(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(IEnumerable<Travel> travels)
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true 
        };
        
        var json = JsonSerializer.Serialize(travels, options);
        await File.WriteAllTextAsync(_filePath, json).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Travel>> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return Enumerable.Empty<Travel>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_filePath).ConfigureAwait(false);
            return JsonSerializer.Deserialize<IEnumerable<Travel>>(json) ?? Enumerable.Empty<Travel>();
        }
        catch (Exception)
        {
            // In a real app, we might want to log this or notify the user
            return Enumerable.Empty<Travel>();
        }
    }
}
