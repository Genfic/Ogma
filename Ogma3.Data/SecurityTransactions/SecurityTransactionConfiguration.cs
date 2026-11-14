namespace Ogma3.Data.SecurityTransactions;

public sealed class SecurityTransactionConfiguration : IEntityTypeConfiguration<SecurityTransaction>
{
	public void Configure(EntityTypeBuilder<SecurityTransaction> builder)
	{
		builder.ToTable("SecurityTransactions");

		builder.HasKey(t => t.Id);

		builder.Property(t => t.Id)
			.HasMaxLength(64);

		builder.Property(t => t.SessionId)
			.IsRequired()
			.HasMaxLength(64);

		builder.Property(t => t.SecurityStamp)
			.IsRequired();

		builder.Property(t => t.ProposedSecret)
			.HasMaxLength(512);

		builder.Property(t => t.Target)
			.HasMaxLength(512);

		builder.HasIndex(t => t.UserId);

		builder.HasIndex(t => new { t.UserId, t.SessionId });

		// NOTE: intentionally no FK to OgmaUser — keeps the model provider-neutral (testable),
		// and orphaned rows for deleted users are removed by the sweep job
	}
}
