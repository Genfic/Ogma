namespace Ogma3.AppHost.Helpers;

public static class Extensions
{
	extension<T>(IResourceBuilder<T> builder) where T : IResource
	{
		public IResourceBuilder<T> If(
			bool condition,
			Func<IResourceBuilder<T>, IResourceBuilder<T>> action
		)
		{
			return condition
				? action(builder)
				: builder;
		}

		public IResourceBuilder<T> If(
			Func<bool> condition,
			Func<IResourceBuilder<T>, IResourceBuilder<T>> action
		)
		{
			return builder.If(condition(), action);
		}

		public IResourceBuilder<T> IfNot(bool condition, Func<IResourceBuilder<T>, IResourceBuilder<T>> action)
		{
			return builder.If(!condition, action);
		}

		public IResourceBuilder<T> IfNot(
			Func<bool> condition,
			Func<IResourceBuilder<T>, IResourceBuilder<T>> action
		)
		{
			return builder.If(!condition(), action);
		}

	}

	extension<T>(IResourceBuilder<T> builder) where T : IResourceWithEnvironment
	{
		public IResourceBuilder<T> WithEnvironmentVariables(Dictionary<string, IResourceBuilder<ParameterResource>> variables)
		{
			foreach (var (key, value) in variables)
			{
				builder.WithEnvironment(key, value);
			}
			return builder;
		}

		public IResourceBuilder<T> WithEnvironmentVariables(Dictionary<string, string> variables)
		{
			foreach (var (key, value) in variables)
			{
				builder.WithEnvironment(key, value);
			}
			return builder;
		}

		public IResourceBuilder<T> WithEnvironmentVariables(HashSet<string> keys)
		{
			foreach (var key in keys)
			{
				var underscored = key.Replace(":", "__");
				builder.WithEnvironment(underscored, builder.ApplicationBuilder.Configuration[key]);
			}
			return builder;
		}
	}

}