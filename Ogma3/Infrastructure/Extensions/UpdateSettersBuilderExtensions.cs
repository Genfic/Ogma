using Microsoft.EntityFrameworkCore.Query;

namespace Ogma3.Infrastructure.Extensions;

public static class UpdateSettersBuilderExtensions
{
	extension<T>(UpdateSettersBuilder<T> builder)
	{
		public UpdateSettersBuilder<T> If(bool condition, Action<UpdateSettersBuilder<T>> func)
		{
			if (condition)
			{
				func(builder);
			}

			return builder;
		}
	}
}