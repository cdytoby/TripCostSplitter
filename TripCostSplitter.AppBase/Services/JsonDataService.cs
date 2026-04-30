using System.Text.Json;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public abstract class JsonDataService: IDataService
{
    public SettingsDataModel Settings { get; private set; }
    public IEnumerable<Travel> Travels => travelsDict.Values;
    
    protected const string TravelFolderName = "Travels";
    protected const string SettingsFileName = "settings.json";
    
    private Dictionary<string, Travel> travelsDict = [];
    private bool loaded;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly string appDataRootPath;
    
    protected JsonDataService(string _appDataRootPath)
    {
        appDataRootPath = _appDataRootPath;
        jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        EnsureFolder(TravelFolderName);
        Settings = new SettingsDataModel();
    }
    
    public async Task Load()
    {
        if (loaded)
            return;
        await LoadAllTravelsAsync();
        await LoadSettingsAsync();
        loaded = true;
    }
    
    public Travel? GetTravel(string travelId)
    {
        return travelsDict.GetValueOrDefault(travelId);
    }
    
    private void EnsureFolder(string folderName)
    {
        string folderPath = Path.Combine(appDataRootPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
    
    private async Task LoadAllTravelsAsync()
    {
        List<Travel> result = [];
        string folderPath = Path.Combine(appDataRootPath, TravelFolderName);
        if (!Directory.Exists(folderPath))
        {
            return;
        }
        
        IEnumerable<string> files = Directory.EnumerateFiles(folderPath);
        foreach (string fileName in files)
        {
            try
            {
                string json = await File.ReadAllTextAsync(fileName).ConfigureAwait(false);
                Travel? thisTravel = JsonSerializer.Deserialize<Travel>(json);
                if (thisTravel != null)
                    result.Add(thisTravel);
                //todo delete when read fail?
            }
            catch (Exception)
            {
                continue;
            }
        }
        
        travelsDict = result.ToDictionary(t => t.TravelId);
    }
    
    private async Task LoadSettingsAsync()
    {
        string fullPath = Path.Combine(appDataRootPath, SettingsFileName);
        if (!File.Exists(fullPath))
        {
            return;
        }
        
        try
        {
            string json = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
            Settings = JsonSerializer.Deserialize<SettingsDataModel>(json) ?? Settings;
        }
        catch
        {
            // ignored
        }
    }
    
    private async Task SaveFileAsync(string relativeFilePath, string content)
    {
        string fullPath = Path.Combine(appDataRootPath, relativeFilePath);
        await File.WriteAllTextAsync(fullPath, content).ConfigureAwait(false);
    }
    
    public async Task SaveTravelAsync(Travel newTravel)
    {
        string travelId = newTravel.TravelId;
        travelsDict[travelId] = newTravel;
        string json = JsonSerializer.Serialize(newTravel, jsonOptions);
        string fileName = newTravel.TravelId + ".json";
        await SaveFileAsync(Path.Combine(TravelFolderName, fileName), json);
    }
    
    public async Task SaveAllTravelsAsync()
    {
        foreach (Travel travel in travelsDict.Values)
        {
            await SaveTravelAsync(travel);
        }
    }
    
    public Task DeleteTravelAsync(string travelId)
    {
        Travel travel = travelsDict[travelId];
        string fileName = travel.TravelId + ".json";
        string fullPath = Path.Combine(appDataRootPath, TravelFolderName, fileName);
        File.Delete(fullPath);
        
        return Task.CompletedTask;
    }
    
    public async Task SaveSettingsAsync()
    {
        string json = JsonSerializer.Serialize(Settings, jsonOptions);
        await SaveFileAsync(SettingsFileName, json);
    }
}