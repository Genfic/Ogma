using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ogma3.Data;
using Ogma3.Data.Stories;
using Ogma3.Infrastructure.Extensions;

namespace Ogma3.Tests.Infrastructure.Extensions;

public sealed class QueryableExtensionsTest
{
	private static readonly IQueryable<int> Numbers = Enumerable.Range(1, 10).AsQueryable();

	private static AppDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql("Host=localhost;Database=ogma;Username=ogma;Password=ogma")
			.Options;

		var services = new ServiceCollection();
		services.AddSingleton(options);
		using var provider = services.BuildServiceProvider();

		return ActivatorUtilities.CreateInstance<AppDbContext>(provider);
	}

	[Test]
	public async Task TestWhereIfWithTrueConditionFilters()
	{
		var result = Numbers.WhereIf(x => x > 5, true).ToArray();
		await Assert.That(result).IsEquivalentTo([6, 7, 8, 9, 10]);
	}

	[Test]
	public async Task TestWhereIfWithFalseConditionDoesNotFilter()
	{
		var result = Numbers.WhereIf(x => x > 5, false).ToArray();
		await Assert.That(result).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
	}

	[Test]
	public async Task TestPaginateFirstPage()
	{
		var page = Numbers.Paginate(1, 4).ToArray();
		await Assert.That(page).IsEquivalentTo([1, 2, 3, 4]);
	}

	[Test]
	public async Task TestPaginateReturnsRequestedPage()
	{
		var page = Numbers.Paginate(2, 4).ToArray();
		await Assert.That(page).IsEquivalentTo([5, 6, 7, 8]);
	}

	[Test]
	public async Task TestPaginateLastPageIsShort()
	{
		var page = Numbers.Paginate(3, 4).ToArray();
		await Assert.That(page).IsEquivalentTo([9, 10]);
	}

	[Test]
	public async Task TestPaginateWithPageLessThanOneThrows()
	{
		await Assert.That(() => Numbers.Paginate(0, 4)).Throws<ArgumentOutOfRangeException>();
	}

	[Test]
	public async Task TestPaginateWithPerPageLessThanOneThrows()
	{
		await Assert.That(() => Numbers.Paginate(1, 0)).Throws<ArgumentOutOfRangeException>();
	}

	[Test]
	public async Task TestBlacklistForLoggedInUser()
	{
		using var ctx = CreateContext();

		var result = Array.Empty<Story>().AsQueryable().Blacklist(ctx, 42L);

		await Assert.That(result).IsNotNull();
	}

	[Test]
	public async Task TestBlacklistForAnonymousUser()
	{
		using var ctx = CreateContext();

		var result = Array.Empty<Story>().AsQueryable().Blacklist(ctx, null);

		await Assert.That(result).IsNotNull();
	}
}