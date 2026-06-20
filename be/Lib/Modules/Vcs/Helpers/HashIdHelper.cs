using Core.Storage;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Vcs.Helpers;

public static class HashIdHelper
{
    public static HashId Parse(string hex)
    {
        try
        {
            return new HashId(Convert.FromHexString(hex));
        }
        catch (FormatException ex)
        {
            throw new BadRequestException($"'{hex}' is not a valid hex string", ex);
        }
    }

    public static HashId? ParseNullable(string? hex)
    {
        return hex is null ? null : Parse(hex);
    }
}