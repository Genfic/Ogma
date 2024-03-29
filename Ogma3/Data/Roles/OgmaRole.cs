using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Users;

namespace Ogma3.Data.Roles;

public class OgmaRole : IdentityRole<long>
{
	public new string Name
	{
		get => base.Name ?? "";
		set => base.Name = value;
	}
	public new string NormalizedName
	{
		get => base.NormalizedName ?? "";
		set => base.NormalizedName = value;
	}
	public bool IsStaff { get; set; }
	public string? Color { get; set; }
	public byte? Order { get; set; }
	public IEnumerable<OgmaUser> Users { get; set; } = [];

	public class OgmaRoleConfig : IEntityTypeConfiguration<OgmaRole>
	{
		public void Configure(EntityTypeBuilder<OgmaRole> builder)
		{
			builder.Property(r => r.IsStaff)
				.IsRequired()
				.HasDefaultValue(false);
			builder.Property(r => r.Color)
				.HasDefaultValue(null);
			builder.Property(r => r.Order)
				.HasDefaultValue(null);
		}
	}

	public OgmaRole Normalize()
	{
		NormalizedName = Name.ToUpperInvariant().Normalize();
		return this;
	}
}