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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new FixedWindowRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new SlidingWindowRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new TokenBucketRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			    return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new ConcurrencyLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);

			    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new FixedWindowRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);

			    return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new SlidingWindowRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);

			    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new TokenBucketRateLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
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
		    options.AddPolicy(policyName, context =>
		    {
			    var partitionKey = partitionKeySelector(context);

			    return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ =>
			    {
				    var limiterOptions = new ConcurrencyLimiterOptions();
				    configureOptions(limiterOptions);
				    return limiterOptions;
			    });
		    });

		    return options;
	    }
    }

}