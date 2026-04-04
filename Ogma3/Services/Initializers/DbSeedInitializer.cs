using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Data.Blogposts;
using Ogma3.Data.Chapters;
using Ogma3.Data.CommentsThreads;
using Ogma3.Data.Icons;
using Ogma3.Data.Images;
using Ogma3.Data.Quotes;
using Ogma3.Data.Ratings;
using Ogma3.Data.Roles;
using Ogma3.Data.Stories;
using Ogma3.Data.Tags;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Constants;
using Ogma3.Services.UserService;
using Utils.Extensions;

namespace Ogma3.Services.Initializers;

[UsedImplicitly]
public sealed class DbSeedInitializer : IHostedLifecycleService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<DbSeedInitializer> _logger;
	private readonly JsonData _data;

	public DbSeedInitializer(IServiceScopeFactory scopeFactory, ILogger<DbSeedInitializer> logger)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;

		using var sr = new StreamReader("seed.json5");
		var data = JsonSerializer.Deserialize(sr.ReadToEnd(), JsonDataContext.Default.JsonData);

		if (data is not null)
		{
			_data = data;
		}
		else
		{
			_logger.LogCritical("Could not read seed.json5 file to seed the database");
			throw new NullReferenceException("Json data was null");
		}
	}

	// Runs before StartAsync of ANY hosted service — guaranteed to complete first
	public async Task StartingAsync(CancellationToken cancellationToken)
	{
		await using var scope = _scopeFactory.CreateAsyncScope();
		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		var userManager = scope.ServiceProvider.GetRequiredService<OgmaUserManager>();
		var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
		var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

		_logger.LogInformation("Async initialization started.");

		var timer = new Stopwatch();
		timer.Start();

		await Time(() => SeedRoles(context), nameof(SeedRoles));
		await Time(() => SeedAdmin(context, userManager, userService), nameof(SeedAdmin));
		await Time(() => SeedUsers(context, userService), nameof(SeedUsers));
		await Time(() => SeedRatings(context), nameof(SeedRatings));
		await Time(() => SeedIcons(context), nameof(SeedIcons));
		await Time(() => SeedQuotes(context, clientFactory), nameof(SeedQuotes));
		var tagIds = await Time(() => SeedTags(context), nameof(SeedTags));
		var storyIds = await Time(() => SeedStories(context), nameof(SeedStories));
		await Time(() => SeedStoryTags(context, storyIds, tagIds), nameof(SeedStoryTags));
		await Time(() => SeedBlogposts(context, storyIds), nameof(SeedBlogposts));

		timer.Stop();
		_logger.LogInformation("Async initialization took {Time} ms", timer.ElapsedMilliseconds);
	}

	// Required interface stubs — no work needed at these lifecycle points
	public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	private static async Task SeedRoles(ApplicationDbContext context)
	{
		if (await Any<OgmaRole>(context)) return;
		var roles = new[]
		{
			new OgmaRole { Name = RoleNames.Admin, IsStaff = true, Color = "#ffaa00", Order = byte.MaxValue }.Normalize(),
			new OgmaRole { Name = RoleNames.Moderator, IsStaff = true, Color = "#aaff00", Order = byte.MaxValue - 5 }.Normalize(),
			new OgmaRole { Name = RoleNames.Helper, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 10 }.Normalize(),
			new OgmaRole { Name = RoleNames.Reviewer, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 15 }.Normalize(),
			new OgmaRole { Name = RoleNames.Supporter, IsStaff = false, Color = "#ffdd11", Order = byte.MaxValue - 20 }.Normalize(),
		};

		await BulkUpsert(context, context.Roles, roles, r => r.NormalizedName);
	}

	private const string Email = "admin@genfic.net";

	private async Task SeedAdmin(ApplicationDbContext context, OgmaUserManager userManager, IUserService userService)
	{
		var exists = await userManager.FindByEmailAsync(Email);
		if (exists is not null) return;

		var password = RandomPassword();

		var result = await userService.CreateAsync("Angius", Email, password, true);
		if (result.Succeeded)
		{
			var adminRole = await context.Roles.SingleOrDefaultAsync(r => r.Name == RoleNames.Admin);
			if (adminRole is null) throw new NullReferenceException("Admin role does not exist, somehow");

			_logger.LogCritical("Admin user created with password {Password}. Change it ASAP.", password);

			result.User.Roles.Add(adminRole);
			await context.SaveChangesAsync();
		}
		else
		{
			_logger.LogCritical("Creating admin failed with errors: {Errors}", result.Errors.Select(e => e.Description));
		}
	}

	private async Task SeedRatings(ApplicationDbContext context)
	{
		await BulkUpsert(context, context.Ratings, _data.Ratings, r => r.Name);
	}

	private async Task SeedIcons(ApplicationDbContext context)
	{
		var icons = _data.Icons.Select(s => new Icon { Name = s });
		await BulkUpsert(context, context.Icons, icons, i => i.Name);
	}

	private async Task SeedQuotes(ApplicationDbContext context, IHttpClientFactory clientFactory)
	{
		if (await context.Quotes.AnyAsync()) return;

		using var hc = clientFactory.CreateClient();
		var json = await hc.GetFromJsonAsync(_data.QuotesUrl, JsonQuoteContext.Default.JsonQuoteArray);

		var quotes = json?.Select(q => new Quote { Body = q.Quote, Author = q.Author });
		if (quotes is null) return;

		context.Quotes.AddRange(quotes);
		await context.SaveChangesAsync();
	}

	private async Task SeedUsers(ApplicationDbContext context, IUserService userService)
	{
		if (await Any<OgmaUser>(context, u => u.Email != Email)) return;

		var avatars = new Faker<Image>()
			.RuleFor(i => i.Url, f => f.Internet.Avatar());

		var usersGenerator = new Faker<OgmaUser>()
			.RuleFor(u => u.UserName, f => f.Internet.UserNameUnicode())
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.Bio, f => f.Lorem.Paragraph())
			.RuleFor(u => u.EmailConfirmed, true)
			.RuleFor(u => u.CommentThread, _ => new CommentThread())
			.RuleFor(u => u.RegistrationDate, f => f.Date.PastOffset().ToUniversalTime())
			.RuleFor(u => u.Title, f => f.Random.String2(f.Random.Int(5, 20)).OrNull(f, 0.6f));

		var users = usersGenerator.Generate(10);
		foreach (var user in users)
		{
			var avatar = avatars.Generate();

			context.Images.Add(avatar);
			await context.SaveChangesAsync();

			user.AvatarId = avatar.Id;

			var password = RandomPassword();
			var result = await userService.CreateAsync(user, password);

			if (result.Succeeded)
			{
				_logger.LogInformation("User {UserName} created with password {Password}.", user.UserName, password);
			}
		}
	}

	private static async Task<(long, ETagNamespace?)[]> SeedTags(ApplicationDbContext context)
	{
		var tags = new List<Tag>
			{
				new() { Name = "Comedy", Namespace = ETagNamespace.Genre },
				new() { Name = "Horror", Namespace = ETagNamespace.Genre },
				new() { Name = "Romance", Namespace = ETagNamespace.Genre },
				new() { Name = "Psychological", Namespace = ETagNamespace.Genre },
				new() { Name = "Slice of Life", Namespace = ETagNamespace.Genre },
				new() { Name = "Parody", Namespace = ETagNamespace.Genre },
				new() { Name = "Drama", Namespace = ETagNamespace.Genre },
				new() { Name = "Thriller", Namespace = ETagNamespace.Genre },
				new() { Name = "Sci-Fi", Namespace = ETagNamespace.Genre },
				new() { Name = "Fantasy", Namespace = ETagNamespace.Genre },

				new() { Name = "Gore", Namespace = ETagNamespace.ContentWarning },
				new() { Name = "Body Horror", Namespace = ETagNamespace.ContentWarning },
				new() { Name = "Sex", Namespace = ETagNamespace.ContentWarning },
				new() { Name = "Self-harm", Namespace = ETagNamespace.ContentWarning },
				new() { Name = "Mental Illness", Namespace = ETagNamespace.ContentWarning },

				new() { Name = "My Little Pony", Namespace = ETagNamespace.Franchise },
				new() { Name = "The Care Bears", Namespace = ETagNamespace.Franchise },
				new() { Name = "The Smurfs", Namespace = ETagNamespace.Franchise },
				new() { Name = "Fast and Furious", Namespace = ETagNamespace.Franchise },

				new() { Name = "Slow Burn" },
				new() { Name = "Oneshot" },
				new() { Name = "Aliens" },
				new() { Name = "Elves" },
				new() { Name = "Orks" },
				new() { Name = "Enemies to Lovers" },
				new() { Name = "Lovers to Enemies" },
				new() { Name = "Space" },
				new() { Name = "Magic" },
				new() { Name = "RPG Mechanics" },
				new() { Name = "Videogames" },
			}
			.Select(t => new Tag
			{
				Name = t.Name,
				Slug = t.Name.Friendlify().ToUpper(),
				Namespace = t.Namespace,
			})
			.ToArray();

		await BulkUpsert(context, context.Tags, tags, t => t.Name);

		return tags.Select(t => (t.Id, t.Namespace)).ToArray();
	}

	private static async Task<long[]> SeedStories(ApplicationDbContext context)
	{
		if (await Any<Story>(context)) return [];

		var userIds = await context.Users.Select(u => u.Id).ToListAsync();
		var ratings = await context.Ratings.Select(r => r.Id).ToListAsync();

		var coverGenerator = new Faker<Image>()
			.RuleFor(i => i.Url, f => f.Image.PicsumUrl());

		var storiesGenerator = new Faker<Story>()
			.RuleFor(s => s.Title, f => f.WaffleTitle())
			.RuleFor(s => s.Slug, (_, s) => s.Title.Friendlify().ToUpper())
			.RuleFor(s => s.AuthorId, f => f.PickRandom(userIds))
			.RuleFor(s => s.Description, f => f.WaffleMarkdown(paragraphs: f.Random.Int(1, 3), includeHeading: false))
			.RuleFor(s => s.CreationDate, f => f.Date.PastOffset().ToUniversalTime())
			.RuleFor(s => s.PublicationDate,
				(f, s) => f.Date.BetweenOffset(s.CreationDate, DateTimeOffset.UtcNow).ToUniversalTime().OrNull(f, 0.2f))
			.RuleFor(s => s.Hook, f => f.WaffleText(includeHeading: false).Trim(CTConfig.Story.MaxHookLength))
			.RuleFor(s => s.RatingId, f => f.PickRandom(ratings))
			.RuleFor(s => s.Status, f => f.PickRandom<EStoryStatus>())
			.RuleFor(s => s.Cover, f => coverGenerator.Generate().OrNull(f, 0.2f))
			.RuleFor(s => s.Chapters, (f, s) => {
				var order = 0u;
				var chapterGenerator = new Faker<Chapter>()
					.RuleFor(c => c.Title, cf => cf.WaffleTitle())
					.RuleFor(c => c.Body, cf => cf.WaffleMarkdown(f.Random.Int(10, 30), false))
					.RuleFor(c => c.PublicationDate, cf => s.PublicationDate is {} pd
						? cf.Date.BetweenOffset(pd, DateTimeOffset.UtcNow).ToUniversalTime()
						: null)
					.RuleFor(c => c.CommentThread, _ => new CommentThread())
					.RuleFor(c => c.Order, () => order++)
					.RuleFor(c => c.CreationDate,
						(cf, c) => cf.Date.BetweenOffset(s.CreationDate, c.PublicationDate ?? DateTimeOffset.UtcNow).ToUniversalTime())
					.RuleFor(c => c.WordCount, (_, c) => c.Body.Words())
					.RuleFor(c => c.Slug, (_, c) => c.Title.Friendlify().ToUpper())
					.RuleFor(c => c.StartNotes,
						cf => cf.WaffleMarkdown(includeHeading: false).Trim(CTConfig.Chapter.MaxNotesLength).OrNull(cf, 0.7f))
					.RuleFor(c => c.EndNotes,
						cf => cf.WaffleMarkdown(includeHeading: false).Trim(CTConfig.Chapter.MaxNotesLength).OrNull(cf, 0.7f));
				return chapterGenerator.Generate(f.Random.Int(1, 5));
			})
			.RuleFor(s => s.ChapterCount, (_, s) => s.Chapters.Count)
			.RuleFor(s => s.WordCount, (_, s) => s.Chapters.Sum(c => c.WordCount));

		var stories = storiesGenerator.Generate(50);

		context.Stories.AddRange(stories);
		await context.SaveChangesAsync();

		return stories.Select(s => s.Id).ToArray();
	}

	private static async Task SeedStoryTags(ApplicationDbContext context, long[] storyIds, (long, ETagNamespace?)[] tags)
	{
		if (await Any<StoryTag>(context)) return;

		var genreTags = tags.Where(g => g.Item2 == ETagNamespace.Genre).ToArray();
		var otherTags = tags.Where(g => g.Item2 != ETagNamespace.Genre).ToArray();

		var storyTags = new List<StoryTag>();
		foreach (var storyId in storyIds)
		{
			var randomGenres = Random.Shared.GetItems(genreTags, Random.Shared.Next(1, 5));
			var randomOther = Random.Shared.GetItems(otherTags, Random.Shared.Next(5, 10));

			HashSet<long> tagIds = [..randomGenres.Select(t => t.Item1), ..randomOther.Select(t => t.Item1)];

			storyTags.AddRange(tagIds.Select(tagId => new StoryTag
			{
				StoryId = storyId,
				TagId = tagId,
			}));
		}

		context.StoryTags.AddRange(storyTags);
		await context.SaveChangesAsync();
	}

	private static async Task SeedBlogposts(ApplicationDbContext context, long[] storyIds)
	{
		if (await Any<Blogpost>(context)) return;

		var userIds = await context.Users.Select(u => u.Id).ToListAsync();
		var chapterIds = await context.Chapters.Select(c => c.Id).ToListAsync();

		var blogpostGenerator = new Faker<Blogpost>()
			.RuleFor(b => b.Title, f => f.WaffleTitle())
			.RuleFor(b => b.Slug, (_, b) => b.Title.Friendlify().ToUpper())
			.RuleFor(b => b.AuthorId, f => f.PickRandom(userIds))
			.RuleFor(b => b.Body, f => f.WaffleMarkdown(f.Random.Int(5, 30), f.Random.Bool()))
			.RuleFor(b => b.CreationDate, f => f.Date.PastOffset().ToUniversalTime())
			.RuleFor(b => b.PublicationDate,
				(f, b) => f.Date.BetweenOffset(b.CreationDate, DateTimeOffset.UtcNow).ToUniversalTime().OrNull(f, 0.2f))
			.RuleFor(b => b.Hashtags, f => f.Lorem.Words())
			.RuleFor(b => b.CommentThread, _ => new CommentThread())
			.RuleFor(b => b.WordCount, (_, b) => b.Body.Words())
			.RuleFor(b => b.AttachedStoryId, f => f.PickRandom(storyIds).OrNull(f, 0.9f))
			.RuleFor(b => b.AttachedChapterId, (f, b) => b.AttachedStoryId is null
				? f.PickRandom(chapterIds).OrNull(f, 0.9f)
				: null);

		var blogposts = blogpostGenerator.Generate(60);
		context.Blogposts.AddRange(blogposts);
		await context.SaveChangesAsync();
	}

	private static async Task BulkUpsert<TEntity>(
		ApplicationDbContext context,
		DbSet<TEntity> source,
		IEnumerable<TEntity> entries,
		Expression<Func<TEntity, object?>> extractor
	) where TEntity : class
	{
		var existing = await source.Select(extractor).ToListAsync();
		var exceptor = extractor.Compile();
		var toAdd = entries.ExceptBy(existing, exceptor);

		source.AddRange(toAdd);
		await context.SaveChangesAsync();
	}

	private static async Task<bool> Any<T>(
		ApplicationDbContext context,
		Expression<Func<T, bool>>? predicate = null
	) where T : class
	{
		return predicate != null
			? await context.Set<T>().AnyAsync(predicate)
			: await context.Set<T>().AnyAsync();
	}

	private async Task Time(Func<Task> func, string name)
	{
		_logger.LogInformation("▶️ Starting {Name}", name);
		var stopwatch = Stopwatch.StartNew();
		await func();
		stopwatch.Stop();
		_logger.LogInformation("⌚ {Name} executed in {Time}ms", name, stopwatch.ElapsedMilliseconds);
	}

	private async Task<T> Time<T>(Func<Task<T>> func, string name)
	{
		_logger.LogInformation("▶️ Starting {Name}", name);
		var stopwatch = Stopwatch.StartNew();
		var res = await func();
		stopwatch.Stop();
		_logger.LogInformation("⌚ {Name} executed in {Time}ms", name, stopwatch.ElapsedMilliseconds);
		return res;
	}

	private static string RandomPassword()
	{
		var alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()".AsSpan();
		return new string(Random.Shared.GetItems(alpha, 20));
	}
}

public sealed record JsonData(string[] Icons, Rating[] Ratings, string QuotesUrl);

[JsonSerializable(typeof(JsonData))]
[JsonSourceGenerationOptions(AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip)]
[UsedImplicitly]
public sealed partial class JsonDataContext : JsonSerializerContext;

public sealed record JsonQuote(string Quote, string Author);

[JsonSerializable(typeof(JsonQuote[]))]
[UsedImplicitly]
public sealed partial class JsonQuoteContext : JsonSerializerContext;