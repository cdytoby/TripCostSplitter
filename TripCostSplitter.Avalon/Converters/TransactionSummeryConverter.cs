using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Converters;

public class TransactionSummeryConverter: IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2 ||
            values[0] is not Transaction transaction ||
            values[1] is not IEnumerable<Person> allPersons)
            return string.Empty;
        
        Dictionary<string, string> personDict = allPersons.ToDictionary(p => p.Id, p => p.Name);
        StringBuilder sb = new();
        
        sb.Append("Paid by: ");
        for (int i = 0; i < transaction.TransactionData.PayerInfos.Count; i++)
        {
            if (i > 0)
                sb.Append(" | ");
            PayerInfo payerInfo = transaction.TransactionData.PayerInfos[i];
            sb.Append(personDict[payerInfo.PayerId]);
            sb.Append(' ');
            sb.Append(CurrencyService.GetFormattedString(transaction.Currency, payerInfo.Amount));
        }
        
        sb.AppendLine();
        sb.Append("Debited to: ");
        for (int i = 0; i < transaction.RecipientInfos.Count; i++)
        {
            if (i > 0)
                sb.Append(" | ");
            RecipientInfo recipientInfo = transaction.RecipientInfos[i];
            sb.Append(personDict[recipientInfo.RecipientId]);
            sb.Append(' ');
            sb.Append(CurrencyService.GetFormattedString(transaction.Currency, recipientInfo.Amount));
        }
        
        return sb.ToString();
    }
}