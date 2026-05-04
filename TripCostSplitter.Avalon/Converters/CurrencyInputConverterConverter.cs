using System.Globalization;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Converters;

public class CurrencyInputConverterConverter: IValueConverter
{
    /// <summary>
    /// value CurrencyModel => targetType CurrencyInputConverter
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CurrencyModel currencyModel)
            return null;
        CurrencyInputConverter CurrencyTextConverter = new(CurrencyService.GetNumberFormat(currencyModel.Code));
        return CurrencyTextConverter;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}