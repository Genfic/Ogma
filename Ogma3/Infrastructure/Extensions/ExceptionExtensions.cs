using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ogma3.Infrastructure.Extensions;

public static class ExceptionExtensions
{
	extension(Exception ex)
	{
		public bool IsPostgresException(string code)
			=> ex is DbUpdateException { InnerException: PostgresException wrapped }
				? wrapped.SqlState == code
				: ex is PostgresException pex && pex.SqlState == code;
	}
}