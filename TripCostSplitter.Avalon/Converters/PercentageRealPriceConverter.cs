using System.Globalization;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Converters;

public class PercentageRealPriceConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3 ||
            values[0] is not decimal percentValue ||
            values[1] is not decimal totalPrice ||
            values[2] is not CurrencyModel currency)
            return string.Empty;
        
        return CurrencyService.GetFormattedString(currency.Code, totalPrice * percentValue);
    }
}