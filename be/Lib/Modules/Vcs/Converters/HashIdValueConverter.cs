using Core.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lib.Modules.Vcs.Converters;

public class HashIdValueConverter : ValueConverter<HashId, string>
{
    public HashIdValueConverter() : base(
        v => Convert.ToHexString(v.Bytes.ToArray()),
        v => new HashId(Convert.FromHexString(v))
    )
    {
    }
}