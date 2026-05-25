using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public class CurrencyService
{
    public const string KeyUnknown = "UNKNOWN";
    
    private readonly IDataService dataService;
    
    public CurrencyService(IDataService _dataService)
    {
        dataService = _dataService;
    }
    
    public decimal? GetExchangeRate(string fromCurrency, string toCurrency)
    {
        CurrencyExchangeRateModel? model1 = dataService.Settings.CachedExchangeRates.FirstOrDefault(rateModel =>
            rateModel.fromCurrency.Equals(fromCurrency) && rateModel.toCurrency.Equals(toCurrency));
        if (model1 != null)
            return model1.rate;
        
        CurrencyExchangeRateModel? model2 = dataService.Settings.CachedExchangeRates.FirstOrDefault(rateModel =>
            rateModel.fromCurrency.Equals(toCurrency) && rateModel.toCurrency.Equals(fromCurrency));
        if (model2 != null)
            return 1m / model2.rate;
        
        return 0;
    }
    
    public static CurrencyModel[] GetAllCurrencyInfos()
    {
        return knownCurrencies.Values.ToArray();
    }
    
    public static CurrencyModel[] GetCurrencyInfos(IEnumerable<string> keys)
    {
        return knownCurrencies.Values.Where(kvp => keys.Contains(kvp.Code)).ToArray();
    }
    
    public static CurrencyModel? GetCurrencyInfo(string key)
    {
        return knownCurrencies.Values.SingleOrDefault(kvp => key.Equals(kvp.Code));
    }
    
    public static CurrencyModel GetCurrencyInfoFromCultureInfo(CultureInfo cultureInfo)
    {
        return knownCurrencies.Values.SingleOrDefault(
            kvp => kvp.CultureInfoCode.Equals(cultureInfo.Name),
            knownCurrencies["UNKNOWN"]);
    }
    
    public string GetDescription(string key)
    {
        return GetDescription(knownCurrencies[key]);
    }
    
    public static NumberFormatInfo GetNumberFormat(string key)
    {
        NumberFormatInfo format = GetCultureInfo(key).NumberFormat;
        format.CurrencyGroupSeparator = "";
        format.NumberGroupSeparator = "";
        format.PercentGroupSeparator = "";
        return format;
    }
    
    public static string GetFormattedString(string key, decimal currencyValue)
    {
        NumberFormatInfo format = GetNumberFormat(key);
        format.CurrencyGroupSeparator = string.Empty;
        return currencyValue.ToString("C2", format);
    }
    
    public static decimal ParseStringToDecimal(string currencyValueString)
    {
        string input = currencyValueString;
        if (string.IsNullOrEmpty(input))
            return 0m;
        Regex numberPattern = new("[0-9.,]+");
        Match? match = numberPattern.Matches(input).FirstOrDefault();
        if (match == null)
            return 0m;
        string[] splitInput = match.Value.Split(',', '.');
        StringBuilder reconstructBuilder = new();
        for (int i = 0; i < splitInput.Length; i++)
        {
            string s = splitInput[i];
            if (i == splitInput.Length - 1 && i > 0)
                reconstructBuilder.Append('.');
            reconstructBuilder.Append(s);
        }
        
        return decimal.Parse(reconstructBuilder.ToString(), CultureInfo.InvariantCulture);
    }
    
    private static CultureInfo GetCultureInfo(string key)
    {
        return CultureInfo.CreateSpecificCulture(knownCurrencies[key].CultureInfoCode);
    }
    
    private static string GetDescription(CurrencyModel currencyModel)
    {
        return currencyModel.Code + "," + currencyModel.Symbol + "," + currencyModel.Name;
    }
    
    private static readonly Dictionary<string, CurrencyModel> knownCurrencies = new()
    {
        [KeyUnknown] = new CurrencyModel(KeyUnknown, "¤", "Invariant", ""),
        ["USD"] = new CurrencyModel("USD", "$", "United States Dollar", "en-US"),
        ["EUR"] = new CurrencyModel("EUR", "€", "Euro", "de-DE"),
        ["GBP"] = new CurrencyModel("GBP", "£", "British Pound Sterling", "en-GB"),
        ["JPY"] = new CurrencyModel("JPY", "¥", "Japanese Yen", "ja-JP"),
        ["CAD"] = new CurrencyModel("CAD", "$", "Canadian Dollar", "en-CA"),
        ["AUD"] = new CurrencyModel("AUD", "$", "Australian Dollar", "en-AU"),
        ["CHF"] = new CurrencyModel("CHF", "CHF", "Swiss Franc", "de-CH"),
        ["CNY"] = new CurrencyModel("CNY", "¥", "Chinese Yuan", "zh-CN"),
        ["HKD"] = new CurrencyModel("HKD", "$", "Hong Kong Dollar", "zh-HK"),
        ["SGD"] = new CurrencyModel("SGD", "$", "Singapore Dollar", "en-SG"),
        ["KRW"] = new CurrencyModel("KRW", "₩", "South Korean Won", "ko-KR"),
        ["INR"] = new CurrencyModel("INR", "₹", "Indian Rupee", "hi-IN"),
        ["BRL"] = new CurrencyModel("BRL", "R$", "Brazilian Real", "pt-BR"),
        ["MXN"] = new CurrencyModel("MXN", "$", "Mexican Peso", "es-MX"),
        ["TRY"] = new CurrencyModel("TRY", "₺", "Turkish Lira", "tr-TR"),
        ["RUB"] = new CurrencyModel("RUB", "₽", "Russian Ruble", "ru-RU"),
        ["ZAR"] = new CurrencyModel("ZAR", "R", "South African Rand", "en-ZA"),
        ["NZD"] = new CurrencyModel("NZD", "$", "New Zealand Dollar", "en-NZ"),
        ["IDR"] = new CurrencyModel("IDR", "Rp", "Indonesian Rupiah", "id-ID"),
        ["THB"] = new CurrencyModel("THB", "฿", "Thai Baht", "th-TH")
    };
}