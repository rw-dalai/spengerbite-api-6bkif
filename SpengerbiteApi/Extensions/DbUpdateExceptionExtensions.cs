using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SpengerbiteApi.Extensions;

public static class DbUpdateExceptionExtensions
{
    // SQLite extended error code for UNIQUE constraint violation
    // See: https://www.sqlite.org/rescode.html#constraint_unique
    private const int SqliteConstraintUnique = 2067;

    /// <summary>
    /// Returns true if the DbUpdateException was caused by a UNIQUE constraint violation in SQLite.
    /// SQLite wraps constraint errors in a generic DbUpdateException.
    /// </summary>
    public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
        => ex.InnerException is SqliteException sqliteEx
           && sqliteEx.SqliteExtendedErrorCode == SqliteConstraintUnique;
}
