#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Stories;
using Ogma3.Data.Users;
using Ogma3.Infrastructure.Converters;

namespace Ogma3.Data.Chapters;

public class ChaptersRead
{
	public Story Story { get; init; }
	public long StoryId { get; init; }
	public OgmaUser User { get; init; }
	public long UserId { get; init; }
	public HashSet<long> Chapters { get; init; }

	public class Configuration : IEntityTypeConfiguration<ChaptersRead>
	{
		public void Configure(EntityTypeBuilder<ChaptersRead> builder)
		{
			builder
				.HasKey(cr => new { cr.StoryId, cr.UserId });

			builder
				.Property(cr => cr.Chapters)
				.HasConversion(new NpgHashsetConverter<long>()) // BUG: This bitch broken
				.Metadata.SetValueComparer(new ValueComparer<HashSet<long>>(
					(a, b) => a.SetEquals(b),
					l => l.Aggregate(0, (i, l1) => HashCode.Combine(i, l1.GetHashCode())),
					h => h.ToHashSet()
				));
		}
	}
}