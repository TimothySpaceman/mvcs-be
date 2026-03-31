using System.Data.Common;
using Npgsql;

namespace Lib.Shared.Utils;

public static class ExceptionExtensions
{
    public static bool IsConflictException(this DbException exception)
    {
        return exception.InnerException switch
        {
            PostgresException pgEx => pgEx.SqlState == "23505",
            _ => false
        };
    }
}