using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.NTest;

public class CurrencyTest
{
    [TestCase("USD", 5.6789, "$5.68")]
    [TestCase("EUR", 3215.6789, "3215,68 €")]
    [TestCase("CNY", 3215.6789, "¥3215.68")]
    public void CheckCurrencyFormat(string key, decimal value, string expectedResult)
    {
        CurrencyService service = TestSetup.ServiceProvider.GetService<CurrencyService>()!;
        
        string result = CurrencyService.GetFormattedString(key, value);
        Console.WriteLine(result);
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
    [TestCase("USD", "5.68", 5.68)]
    [TestCase("EUR", "3215,68", 3215.68)]
    [TestCase("EUR", "3,215.68", 3215.68)]
    [TestCase("CNY", "3215.68", 3215.68)]
    public void CheckCurrencyInput(string key, string value, decimal expectedResult)
    {
        CurrencyService service = TestSetup.ServiceProvider.GetService<CurrencyService>()!;
        
        decimal result = CurrencyService.ParseStringToDecimal(value);
        Console.WriteLine(result);
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}