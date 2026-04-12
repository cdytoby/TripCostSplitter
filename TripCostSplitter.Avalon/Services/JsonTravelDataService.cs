using System.Text.Json;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Services;

public abstract class JsonTravelDataService(string appDataRootPath): ITravelDataService
{
    protected const string FolderName = "Travels";
    
    private JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true
    };
    
    public async Task SaveAsync(Travel travel)
    {
        string json = JsonSerializer.Serialize(travel, jsonOptions);
        string fileName = travel.TravelId + ".json";
        string folderPath = Path.Combine(appDataRootPath, FolderName);
        string filePath = Path.Combine(folderPath, fileName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        await File.WriteAllTextAsync(filePath, json).ConfigureAwait(false);
    }
    
    public async Task SaveAllAsync(IEnumerable<Travel> travels)
    {
        foreach (Travel travel in travels)
        {
            await SaveAsync(travel);
        }
    }
    
    public async Task<IEnumerable<Travel>> LoadAllAsync()
    {
        List<Travel> result = [];
        string folderPath = Path.Combine(appDataRootPath, FolderName);
        if (!Directory.Exists(folderPath))
        {
            return result;
        }
        
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath);
            foreach (string fileName in files)
            {
                string json = await File.ReadAllTextAsync(fileName);
                Travel? thisTravel = JsonSerializer.Deserialize<Travel>(json);
                if (thisTravel != null)
                    result.Add(thisTravel);
                //todo delete when read fail?
            }
        }
        catch (Exception)
        {
            //Todo: In a real app, we might want to log this or notify the user
            return result;
        }
        
        return result;
    }
}