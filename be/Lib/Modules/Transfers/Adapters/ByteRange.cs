using System.Globalization;
using System.Text.RegularExpressions;

namespace Lib.Modules.Transfers.Adapters;

public record ByteRange(long? Start, long? End)
{
    public static ByteRange FromHeaderValue(string headerValue)
    {
        var match = Regex.Match(headerValue, @"^(?:bytes=)?(\d*)-(\d*)$");
        long? start = long.TryParse(match.Groups[1].Value, out var startValue) ? startValue : null;
        long? end = long.TryParse(match.Groups[2].Value, out var endValue) ? endValue : null;
        return new ByteRange(start, end);
    }

    public string ToHeaderValue()
    {
        return string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", Start, End);
    }
};