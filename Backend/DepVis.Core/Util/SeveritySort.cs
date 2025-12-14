namespace DepVis.Core.Util;

public class SeveritySort
{
    public static readonly Dictionary<string, int> SeverityRank = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["None"] = 0,
        ["low"] = 1,
        ["medium"] = 2,
        ["high"] = 3,
        ["critical"] = 4,
    };
}
