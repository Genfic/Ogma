using System.IO.Hashing;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Ogma3.Infrastructure.Extensions;

public static class RateLimiterOptionsExtensions
{
    extension(RateLimiterOptions options)
    {
	    /// <summary>
	    /// Adds a partitioned fixed window rate limiter using the client's IP address as the partition key.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedFixedWindowLimiter(
		    string policyName,
		    Action<FixedWindowRateLimiterOptions> configureOptions)
	    {
		    var limiterOptions = new FixedWindowRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.Hash() ?? 0;
			    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned sliding window rate limiter using the client's IP address as the partition key.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedSlidingWindowLimiter(
		    string policyName,
		    Action<SlidingWindowRateLimiterOptions> configureOptions)
	    {
		    var limiterOptions = new SlidingWindowRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.Hash() ?? 0;
			    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned token bucket rate limiter using the client's IP address as the partition key.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedTokenBucketLimiter(
		    string policyName,
		    Action<TokenBucketRateLimiterOptions> configureOptions)
	    {
		    var limiterOptions = new TokenBucketRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.Hash() ?? 0;
			    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned concurrency rate limiter using the client's IP address as the partition key.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedConcurrencyLimiter(
		    string policyName,
		    Action<ConcurrencyLimiterOptions> configureOptions)
	    {
		    var limiterOptions = new ConcurrencyLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.Hash() ?? 0;
			    return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned fixed window rate limiter with a custom partition key selector.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedFixedWindowLimiter<TPartitionKey>(
		    string policyName,
		    Func<HttpContext, TPartitionKey> partitionKeySelector,
		    Action<FixedWindowRateLimiterOptions> configureOptions) where TPartitionKey : notnull
	    {
		    var limiterOptions = new FixedWindowRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);
			    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned sliding window rate limiter with a custom partition key selector.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedSlidingWindowLimiter<TPartitionKey>(
		    string policyName,
		    Func<HttpContext, TPartitionKey> partitionKeySelector,
		    Action<SlidingWindowRateLimiterOptions> configureOptions) where TPartitionKey : notnull
	    {
		    var limiterOptions = new SlidingWindowRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);
			    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned token bucket rate limiter with a custom partition key selector.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedTokenBucketLimiter<TPartitionKey>(
		    string policyName,
		    Func<HttpContext, TPartitionKey> partitionKeySelector,
		    Action<TokenBucketRateLimiterOptions> configureOptions) where TPartitionKey : notnull
	    {
		    var limiterOptions = new TokenBucketRateLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);
			    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }

	    /// <summary>
	    /// Adds a partitioned concurrency rate limiter with a custom partition key selector.
	    /// </summary>
	    public RateLimiterOptions AddPartitionedConcurrencyLimiter<TPartitionKey>(
		    string policyName,
		    Func<HttpContext, TPartitionKey> partitionKeySelector,
		    Action<ConcurrencyLimiterOptions> configureOptions) where TPartitionKey : notnull
	    {
		    var limiterOptions = new ConcurrencyLimiterOptions();
		    configureOptions(limiterOptions);

		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);
			    return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ => limiterOptions);
		    });

		    return options;
	    }
    }

    // Should we ever run multiple instances, this could cause issues, being different per instance
    private static readonly long Seed = Random.Shared.NextInt64();

    private static long Hash(this IPAddress? ip)
    {
	    if (ip is null) return 0L;

	    Span<byte> bytes = stackalloc byte[16];
	    return ip.TryWriteBytes(bytes, out var written)
		    ? (long)XxHash3.HashToUInt64(bytes[..written], Seed)
		    : 0L;

    }
}