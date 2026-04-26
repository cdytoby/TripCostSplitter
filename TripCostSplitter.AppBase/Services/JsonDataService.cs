using System.Text.Json;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public abstract class JsonDataService: IDataService
{
    protected const string TravelFolderName = "Travels";
    protected const string SettingsFileName = "settings.json";
    
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
        
        return result;
    }
    
    public Task DeleteTravelAsync(Travel travel)
    {
        string fileName = travel.TravelId + ".json";
        string fullPath = Path.Combine(appDataRootPath, TravelFolderName, fileName);
        File.Delete(fullPath);
        
        return Task.CompletedTask;
    }

    public async Task<SettingsDataModel> LoadSettingsAsync()
    {
        string fullPath = Path.Combine(appDataRootPath, SettingsFileName);
        if (!File.Exists(fullPath))
        {
            return new SettingsDataModel();
        }

        try
        {
            string json = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);
            return JsonSerializer.Deserialize<SettingsDataModel>(json) ?? new SettingsDataModel();
        }
        catch
        {
            return new SettingsDataModel();
        }
    }

    public async Task SaveSettingsAsync(SettingsDataModel settings)
    {
        string json = JsonSerializer.Serialize(settings, jsonOptions);
        await SaveFileAsync(SettingsFileName, json);
    }
}