using System.Globalization;
using TripCostSplitter.Avalon.Converters;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.NTest;

[TestFixture]
public class UICurrencyTest
{
    [TestCase("en-us", 5.6789, "$5.68")]
    [TestCase("de-de", 3215.6789, "3.215,68 €")]
    [TestCase("zh-cn", 3215.6789, "¥3,215.68")]
    public void CheckCurrencyFormat(string cultureString, decimal value, string expectedResult)
    {
        CurrencyInputConverter converter = new(CultureInfo.GetCultureInfo(cultureString).NumberFormat);
        string? result = (string?)converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);
        Console.WriteLine(result);
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
    [TestCase("en-us", "$5.68", 5.68)]
    [TestCase("en-us", "3,215.68€", 3215.68)]
    [TestCase("en-us", "3215,68€", 3215.68)]
    [TestCase("en-us", "¥3.215,68", 3215.68)]
    [TestCase("de-de", "¥5.68", 5.68)]
    [TestCase("de-de", "3,215.68€", 3215.68)]
    [TestCase("de-de", "$3215,68", 3215.68)]
    [TestCase("de-de", "3.215,68€", 3215.68)]
    public void CheckCurrencyInput(string cultureString, string value, decimal expectedResult)
    {
        CurrencyInputConverter converter = new(CultureInfo.GetCultureInfo(cultureString).NumberFormat);
        decimal? testResult = (decimal?) converter.Convert(value, typeof(decimal), null, CultureInfo.InvariantCulture);
        
        Assert.That(testResult, Is.EqualTo(expectedResult));
    }
}