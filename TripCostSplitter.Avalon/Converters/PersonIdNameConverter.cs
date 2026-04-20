using System.Globalization;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.Converters;

public class PersonIdNameConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2 || values[0] is not int personId || values[1] is not IEnumerable<Person> allPersons)
            return string.Empty;
        
        return allPersons.FirstOrDefault(p => p.Id.Equals(personId))?.Name;
    }
}