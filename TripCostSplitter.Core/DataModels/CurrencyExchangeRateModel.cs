namespace TripCostSplitter.Core.DataModels;

/// <summary>
/// fromCurrencyMoney * rate = toCurrencyMoney
/// </summary>
/// <param name="fromCurrency"></param>
/// <param name="toCurrency"></param>
/// <param name="rate"></param>
public record CurrencyExchangeRateModel(string fromCurrency, string toCurrency, decimal rate);