namespace Ogma3.Infrastructure.Middleware.RequestTimingMiddleware;

public interface IRequestTimingFeature
{
	DateTimeOffset StartTime { get; init; }
	TimeSpan Duration { get; }
}

public sealed class RequestTimingFeature : IRequestTimingFeature
{
	public required DateTimeOffset StartTime { get; init; }
	public TimeSpan Duration => DateTimeOffset.UtcNow - StartTime;
}