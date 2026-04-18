using System.Text.Json.Serialization;

namespace TripCostSplitter.Core.SplitData;

[JsonDerivedType(typeof(SplitByExactAmount), typeDiscriminator: SplitByExactAmount.Key)]
[JsonDerivedType(typeof(SplitByItemOwnership), typeDiscriminator: SplitByItemOwnership.Key)]
[JsonDerivedType(typeof(SplitByPercentage), typeDiscriminator: SplitByPercentage.Key)]
[JsonDerivedType(typeof(SplitEvenly), typeDiscriminator: SplitEvenly.Key)]
public interface ISplitData
{
}