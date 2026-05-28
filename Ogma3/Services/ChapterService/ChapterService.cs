using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.OgmaConfig;

namespace Ogma3.Services.ChapterService;

[RegisterScoped]
[UsedImplicitly]
public sealed class ChapterService(ApplicationDbContext context, OgmaConfig config)
{
	public async Task<IReadOnlyList<long>> IsPlagiarized(long id, CancellationToken ct = default)
	{
		var signature = await context.Chapters
			.Where(c => c.Id == id)
			.Select(c => c.Signature)
			.FirstOrDefaultAsync(ct);

		if (signature is null)
		{
			return [];
		}

		return await IsPlagiarized(signature, ct);
	}

	public async Task<IReadOnlyList<long>> IsPlagiarized(uint[] signature, CancellationToken ct = default)
	{
		var sig = Array.ConvertAll(signature, x => (int)x);

		var chaps = await context.Chapters
			.FromSql($"""
				SELECT * FROM "Chapters" c
					WHERE c."Signature" && {sig} -- initial overlap check
					AND icount(c."Signature" & {sig})::float / {sig.Length} >= {config.PlagiarismThreshold} -- check for exact overlap (threshold)
				ORDER BY icount(c."Signature" & {sig}) DESC
				""")
			.Select(c => c.Id)
			.ToListAsync(ct);

		return chaps;
	}

	public async Task<IReadOnlyList<long>> IsPlagiarized(uint[] signature, long id, CancellationToken ct = default)
	{
		var sig = Array.ConvertAll(signature, x => (int)x);

		var chaps = await context.Chapters
			.FromSql($"""
			          SELECT * FROM "Chapters" c
			          WHERE c."Id" <> {id}
			            AND c."Signature" && {sig}
			            AND icount(c."Signature" & {sig})::float / {sig.Length}
			                  >= {config.PlagiarismThreshold}
			          ORDER BY icount(c."Signature" & {sig}) DESC
			          """)
			.Select(c => c.Id)
			.ToListAsync(ct);

		return chaps;
	}

}