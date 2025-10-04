using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using Extensions.Hosting.AsyncInitialization;
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
using Utils;
using Utils.Extensions;

namespace Ogma3.Services.Initializers;

[UsedImplicitly]
public sealed class DbSeedInitializer : IAsyncInitializer
{
	private readonly ApplicationDbContext _context;
	private readonly OgmaUserManager _userManager;
	private readonly ILogger<DbSeedInitializer> _logger;
	private readonly IHttpClientFactory _clientFactory;
	private readonly Faker _faker = new();

	private readonly JsonData _data;

	public DbSeedInitializer(
		ApplicationDbContext context,
		OgmaUserManager userManager,
		ILogger<DbSeedInitializer> logger,
		IHttpClientFactory clientFactory
	)
	{
		_context = context;
		_userManager = userManager;
		_logger = logger;
		_clientFactory = clientFactory;

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

	public async Task InitializeAsync(CancellationToken ct)
	{
		_logger.LogInformation("Async initialization started.");

		var timer = new Stopwatch();
		timer.Start();

		await using var transaction = await _context.Database.BeginTransactionAsync(ct);

		await Time(SeedRoles, nameof(SeedRoles));
		await Time(SeedAdmin, nameof(SeedAdmin));
		await Time(SeedUsers, nameof(SeedUsers));
		await Time(SeedRatings, nameof(SeedRatings));
		await Time(SeedIcons, nameof(SeedIcons));
		await Time(SeedQuotes, nameof(SeedQuotes));
		var tagIds = await Time(SeedTags, nameof(SeedTags));
		var storyIds = await Time(SeedStories, nameof(SeedStories));
		await Time(() => SeedStoryTags(storyIds, tagIds), nameof(SeedStoryTags));
		await Time(() => SeedBlogposts(storyIds), nameof(SeedBlogposts));

		await transaction.CommitAsync(ct);

		timer.Stop();
		_logger.LogInformation("Async initialization took {Time} ms", timer.ElapsedMilliseconds);
	}

	private async Task SeedRoles()
	{
		if (await Any<OgmaRole>()) return;
		var roles = new[]
		{
			new OgmaRole { Name = RoleNames.Admin, IsStaff = true, Color = "#ffaa00", Order = byte.MaxValue }.Normalize(),
			new OgmaRole { Name = RoleNames.Moderator, IsStaff = true, Color = "#aaff00", Order = byte.MaxValue - 5 }.Normalize(),
			new OgmaRole { Name = RoleNames.Helper, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 10 }.Normalize(),
			new OgmaRole { Name = RoleNames.Reviewer, IsStaff = true, Color = "#ffdd11", Order = byte.MaxValue - 15 }.Normalize(),
			new OgmaRole { Name = RoleNames.Supporter, IsStaff = false, Color = "#ffdd11", Order = byte.MaxValue - 20 }.Normalize(),
		};

		await BulkUpsert(_context.Roles, roles, r => r.NormalizedName);
	}

	private const string Email = "admin@genfic.net";
	public async Task SeedAdmin()
	{
		var exists = await _userManager.FindByEmailAsync(Email);
		if (exists is not null) return;

		var adminRole = await _context.Roles.SingleOrDefaultAsync(r => r.NormalizedName == RoleNames.Admin);
		if (adminRole is null) throw new NullReferenceException("Admin role does not exist, somehow");

		var password = RandomPassword();
		var user = new OgmaUser
		{
			UserName = "Angius",
			Email = Email,
		};
		var result = await _userManager.CreateAsync(user, password);
		if (result.Succeeded)
		{
			_logger.LogCritical("Admin user created with password {Password}. Change it ASAP.", password);
		}
		else
		{
			_logger.LogCritical("Creating admin failed with errors: {Errors}", result.Errors.Select(e => e.Description));
		}

		var admin = await _context.Users.SingleOrDefaultAsync(u => u.Email == Email);
		if (admin is not null)
		{
			admin.Roles.Add(adminRole);
			admin.Avatar = new Image
			{
				Url = Gravatar.Generate(Email),
			};
		}
		await _context.SaveChangesAsync();
	}

	private async Task SeedRatings()
	{
		await BulkUpsert(_context.Ratings, _data.Ratings, r => r.Name);
	}

	private async Task SeedIcons()
	{
		var icons = _data.Icons.Select(s => new Icon { Name = s });

		await BulkUpsert(_context.Icons, icons, i => i.Name);
	}

	private async Task SeedQuotes()
	{
		if (await _context.Quotes.AnyAsync()) return;

		using var hc = _clientFactory.CreateClient();
		var json = await hc.GetFromJsonAsync(_data.QuotesUrl, JsonQuoteContext.Default.JsonQuoteArray);

		var quotes = json?.Select(q => new Quote { Body = q.Quote, Author = q.Author });
		if (quotes is null) return;

		_context.Quotes.AddRange(quotes);

		await _context.SaveChangesAsync();
	}

	public async Task SeedUsers()
	{
		if (await Any<OgmaUser>()) return;

		var avatars = new Faker<Image>()
			.RuleFor(i => i.Url, f => f.Internet.Avatar());

		var usersGenerator = new Faker<OgmaUser>()
			.RuleFor(u => u.UserName, f => f.Internet.UserNameUnicode())
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.Avatar, _ => avatars.Generate());

		var users = usersGenerator.Generate(10);
		foreach (var user in users)
		{
			var password = RandomPassword();
			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				_logger.LogInformation("User {UserName} created with password {Password}.", user.UserName, password);
			}
		}

		var userList = await _context.Users.ToListAsync();

		foreach (var user in userList)
		{
			user.Bio = _faker.Lorem.Paragraph();
			user.EmailConfirmed = true;
			user.CommentThread = new CommentThread();
			user.RegistrationDate = _faker.Date.PastOffset().ToUniversalTime();
			user.Title = _faker.Random.String2(_faker.Random.Int(5, 20)).OrNull(_faker, 0.6f);
		}

		await _context.SaveChangesAsync();
	}

	public async Task<(long, ETagNamespace?)[]> SeedTags()
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
		}.Select(t => new Tag
		{
			Name = t.Name,
			Slug = t.Name.Friendlify().ToUpper(),
			Namespace = t.Namespace,
		})
		.ToArray();

		await BulkUpsert(_context.Tags, tags, t => t.Name);

		return tags.Select(t => (t.Id, t.Namespace)).ToArray();
	}

	public async Task<long[]> SeedStories()
	{
		if (await Any<Story>()) return [];

		var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
		var ratings = await _context.Ratings.Select(r => r.Id).ToListAsync();

		var coverGenerator = new Faker<Image>()
			.RuleFor(i => i.Url, f => f.Image.PicsumUrl());

		var storiesGenerator = new Faker<Story>()
			.RuleFor(s => s.Title, f => f.WaffleTitle())
			.RuleFor(s => s.Slug, (_, s) => s.Title.Friendlify().ToUpper())
			.RuleFor(s => s.AuthorId, f => f.PickRandom(userIds))
			.RuleFor(s => s.Description, f => f.WaffleMarkdown(paragraphs: f.Random.Int(1, 3), includeHeading: false))
			.RuleFor(s => s.CreationDate, f => f.Date.PastOffset().ToUniversalTime())
			.RuleFor(s => s.PublicationDate, (f, s) => f.Date.BetweenOffset(s.CreationDate, DateTimeOffset.UtcNow).ToUniversalTime().OrNull(f, 0.2f))
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
					.RuleFor(c => c.CommentThread, new CommentThread())
					.RuleFor(c => c.Order, () => order++)
					.RuleFor(c => c.CreationDate, (cf, c) => cf.Date.BetweenOffset(s.CreationDate, c.PublicationDate ?? DateTimeOffset.UtcNow).ToUniversalTime())
					.RuleFor(c => c.WordCount, (_, c) => c.Body.Words())
					.RuleFor(c => c.Slug, (_, c) => c.Title.Friendlify().ToUpper())
					.RuleFor(c => c.StartNotes, cf => cf.WaffleMarkdown(includeHeading: false)
						.Trim(CTConfig.Chapter.MaxNotesLength)
						.OrNull(cf, 0.7f))
					.RuleFor(c => c.EndNotes, cf => cf.WaffleMarkdown(includeHeading: false)
						.Trim(CTConfig.Chapter.MaxNotesLength)
						.OrNull(cf, 0.7f))
					.RuleFor(c => c.CommentThread, new CommentThread());
				return chapterGenerator.Generate(f.Random.Int(1, 5));
			})
			.RuleFor(s => s.ChapterCount, (_, s) => s.Chapters.Count)
			.RuleFor(s => s.WordCount, (_, s) => s.Chapters.Sum(c => c.WordCount));
		var stories = storiesGenerator.Generate(50);

		_context.Stories.AddRange(stories);
		await _context.SaveChangesAsync();

		return stories.Select(s => s.Id).ToArray();
	}

	public async Task SeedStoryTags(long[] storyIds, (long, ETagNamespace?)[] tags)
	{
		if (await Any<StoryTag>()) return;

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

		_context.StoryTags.AddRange(storyTags);
		await _context.SaveChangesAsync();
	}

	public async Task SeedBlogposts(long[] storyIds)
	{
		if (await Any<Blogpost>()) return;

		var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
		var chapterIds = await _context.Chapters.Select(c => c.Id).ToListAsync();

		var blogpostGenerator = new Faker<Blogpost>()
			.RuleFor(s => s.Title, f => f.WaffleTitle())
			.RuleFor(s => s.Slug, (_, s) => s.Title.Friendlify().ToUpper())
			.RuleFor(s => s.AuthorId, f => f.PickRandom(userIds))
			.RuleFor(s => s.Body, f => f.WaffleMarkdown(f.Random.Int(5, 30), f.Random.Bool()))
			.RuleFor(b => b.CreationDate, f => f.Date.PastOffset().ToUniversalTime())
			.RuleFor(b => b.PublicationDate, (f, b) => f.Date.BetweenOffset(b.CreationDate, DateTimeOffset.UtcNow).ToUniversalTime().OrNull(f, 0.2f))
			.RuleFor(b => b.Hashtags, f => f.Lorem.Words())
			.RuleFor(b => b.CommentThread, new CommentThread())
			.RuleFor(b => b.WordCount, (_, b) => b.Body.Words())
			.RuleFor(b => b.AttachedStoryId, f => f.PickRandom(storyIds).OrNull(f, 0.9f))
			.RuleFor(b => b.AttachedChapterId, (f, b) => b.AttachedStoryId is null
				? f.PickRandom(chapterIds).OrNull(f, 0.9f)
				: null);

		var blogposts = blogpostGenerator.Generate(60);
		_context.Blogposts.AddRange(blogposts);
		await _context.SaveChangesAsync();
	}

	private async Task BulkUpsert<TEntity, TKey>(
		DbSet<TEntity> source,
		IEnumerable<TEntity> entries,
		Expression<Func<TEntity, TKey>> extractor
	) where TEntity : class
	{
		var existing = await source
			.Select(extractor)
			.ToListAsync();

		var exceptor = extractor.Compile();
		var toAdd = entries.ExceptBy(existing, exceptor);

		source.AddRange(toAdd);

		await _context.SaveChangesAsync();
	}

	private async Task<bool> Any<T>() where T : class
	{
		var any = await _context.Set<T>().AnyAsync();
		return any;
	}

	private async Task Time(Func<Task> func, string name)
	{
		_logger.LogInformation("Starting {Name}", name);

		var stopwatch = new Stopwatch();
		stopwatch.Start();

		await func();

		stopwatch.Stop();
		_logger.LogInformation("{Name} executed in {Time}ms", name, stopwatch.ElapsedMilliseconds);
	}

	private async Task<T> Time<T>(Func<Task<T>> func, string name)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		var res = await func();

		stopwatch.Stop();
		_logger.LogInformation("{Name} executed in {Time}ms", name, stopwatch.ElapsedMilliseconds);

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