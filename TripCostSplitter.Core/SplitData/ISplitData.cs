using System.Text.Json.Serialization;

namespace TripCostSplitter.Core.SplitData;

[JsonDerivedType(typeof(SplitByExactAmount), typeDiscriminator: "ByExactAmount")]
[JsonDerivedType(typeof(SplitByItemOwnership), typeDiscriminator: "ByItemOwnership")]
[JsonDerivedType(typeof(SplitByPercentage), typeDiscriminator: "ByPercentage")]
[JsonDerivedType(typeof(SplitEvenly), typeDiscriminator: "Evenly")]
public interface ISplitData
{
}