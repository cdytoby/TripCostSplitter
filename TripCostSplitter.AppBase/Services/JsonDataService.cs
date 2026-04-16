using System.Text.Json;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public abstract class JsonDataService: IDataService
{
    protected const string TravelFolderName = "Travels";
    protected const string AccessDataFileName = "access.json";
    
    private JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true
    };
    
    private readonly string appDataRootPath;
    
    protected JsonDataService(string _appDataRootPath)
    {
        appDataRootPath = _appDataRootPath;
        
        EnsureFolder(TravelFolderName);
    }
    
    private void EnsureFolder(string folderName)
    {
        string folderPath = Path.Combine(appDataRootPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
    
    private async Task SaveFileAsync(string relativeFilePath, string content)
    {
        string fullPath = Path.Combine(appDataRootPath, relativeFilePath);
        await File.WriteAllTextAsync(fullPath, content).ConfigureAwait(false);
    }
    
    public async Task SaveAccessData(AccessManagerData accessData)
    {
        await SaveFileAsync(AccessDataFileName, JsonSerializer.Serialize(accessData, jsonOptions));
    }
    
    public async Task<AccessManagerData> LoadAccessData()
    {
        string fullPath = Path.Combine(appDataRootPath, AccessDataFileName);
        AccessManagerData? loadData = null;
        try
        {
            string textData = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
            loadData = JsonSerializer.Deserialize<AccessManagerData>(textData);
        }
        catch
        {
            // ignored
        }
        
        loadData ??= new AccessManagerData();
        
        return loadData;
    }
    
    public async Task SaveTravelAsync(Travel travel)
    {
        string json = JsonSerializer.Serialize(travel, jsonOptions);
        string fileName = travel.TravelId + ".json";
        await SaveFileAsync(Path.Combine(TravelFolderName, fileName), json);
    }
    
    public async Task SaveAllTravelsAsync(IEnumerable<Travel> travels)
    {
        foreach (Travel travel in travels)
        {
            await SaveTravelAsync(travel);
        }
    }
    
    public async Task<IEnumerable<Travel>> LoadAllTravelsAsync()
    {
        List<Travel> result = [];
        string folderPath = Path.Combine(appDataRootPath, TravelFolderName);
        if (!Directory.Exists(folderPath))
        {
            return result;
        }
        
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath);
            foreach (string fileName in files)
            {
                string json = await File.ReadAllTextAsync(fileName).ConfigureAwait(false);
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