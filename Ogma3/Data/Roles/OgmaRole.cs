using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ogma3.Data.Users;

namespace Ogma3.Data.Roles;

public sealed class OgmaRole : IdentityRole<long>
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
	public byte Order { get; set; }
	public IEnumerable<OgmaUser> Users { get; init; } = [];

	public sealed class OgmaRoleConfig : IEntityTypeConfiguration<OgmaRole>
	{
		public void Configure(EntityTypeBuilder<OgmaRole> builder)
		{
			builder.Property(r => r.IsStaff)
				.IsRequired()
				.HasDefaultValue(false);
			builder.Property(r => r.Color)
				.HasMaxLength(7)
				.HasDefaultValue(null);
			builder.Property(r => r.Order)
				.HasDefaultValue(0);
		}
	}

	public OgmaRole Normalize()
	{
		NormalizedName = Name.ToUpperInvariant().Normalize();
		return this;
	}
}