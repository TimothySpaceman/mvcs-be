using System.Text;

namespace Lib.Modules.Transfers.Helpers;

public class TusMetadataHelper
{
    public static string DecodeMetadataValue(string base64Value)
    {
        var bytes = Convert.FromBase64String(base64Value);
        return Encoding.UTF8.GetString(bytes);
    }

    public static Dictionary<string, string> ParseMetadata(string metadata)
    {
        return metadata
            .Split(',')
            .Select(pair => pair.Trim().Split(' '))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0],
                parts => DecodeMetadataValue(parts[1])
            );
    }
}