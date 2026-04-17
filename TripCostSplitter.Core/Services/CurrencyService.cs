using System.Globalization;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public class CurrencyService
{
    public CurrencyModel[] GetAllCurrencyInfos()
    {
        return knownCurrencies.Values.ToArray();
    }
    
    public string GetDescription(string key)
    {
        return GetDescription(knownCurrencies[key]);
    }
    
    public string GetFormattedString(string key, decimal currencyValue)
    {
        NumberFormatInfo format = GetCultureInfo(key).NumberFormat;
        format.CurrencyGroupSeparator = string.Empty;
        return currencyValue.ToString("C", format);
    }
    
    public decimal ParseFormattedString(string key, string currencyValueString)
    {
        bool success1 = decimal.TryParse(
            currencyValueString, NumberStyles.Currency, GetCultureInfo(key), out decimal currencyValue);
        if (success1)
            return currencyValue;
        bool success2 = decimal.TryParse(
            currencyValueString, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
            out decimal currencyValue2);
        if (success2)
            return currencyValue2;
        
        return 0;
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