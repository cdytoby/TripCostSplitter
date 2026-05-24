using System.Globalization;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Converters;

public class CurrencyTextConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [decimal decimalValue, CurrencyModel currency])
            return "NaN";
        
        return CurrencyService.GetFormattedString(currency.Code, decimalValue);
    }
}