using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Converters;

public class CurrencyInputConverter: IValueConverter
{
    private NumberFormatInfo numberFormat;
    
    public CurrencyInputConverter(NumberFormatInfo? _numberFormat = null)
    {
        numberFormat = _numberFormat ?? CultureInfo.InvariantCulture.NumberFormat;
    }
    
    /// <summary>
    /// value string => targetType Decimal
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string input = value?.ToString() ?? string.Empty;
        return CurrencyService.ParseStringToDecimal(input);
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not decimal inputNumber)
            return null;
        return inputNumber.ToString("C2", numberFormat);
    }
}