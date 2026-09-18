using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Data.Helpers;

public static class EntityBuilderExtensions
{
	extension<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
	{
		public IndexBuilder HasPartialIndex(Expression<Func<TEntity, object?>> indexExpression)
		{
			IReadOnlyList<MemberExpression> members = indexExpression.Body switch
			{
				MemberExpression m => [m],
				UnaryExpression { Operand: MemberExpression m, NodeType: ExpressionType.Convert } => [m],
				NewExpression n => n.Arguments
					.Select(a => a switch
					{
						MemberExpression m => m,
						UnaryExpression { Operand: MemberExpression m, NodeType: ExpressionType.Convert } => m,
						_ => throw new NotSupportedException($"Expression '{a}' is not supported"),
					})
					.ToList(),
				_ => throw new NotSupportedException($"Expression '{indexExpression}' is not supported"),
			};

			var columns = members.Select(m => GetColumnName(builder, m.Member));

			var sql = string.Join(" AND ", columns.Select(column => $"\"{column}\" IS NOT NULL"));

			return builder.HasIndex(indexExpression).HasFilter(sql);
		}
		public EntityTypeBuilder<TEntity> HasMaxCardinality<TElement>(Expression<Func<TEntity, TElement>> propertyExpression, int max)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(max);

			if (propertyExpression.Body is not MemberExpression member)
			{
				throw new NotSupportedException($"Expression '{propertyExpression}' is not supported");
			}

			var columnName = GetColumnName(builder, member.Member);

			var type = builder.Metadata.ClrType.Name;
			var propertyName = member.Member.Name;

			var constraintName = $"CK_{type}_{propertyName}_cardinality";
			var sql = $"""cardinality("{columnName}") <= {max}""";

			return builder.ToTable(t => t.HasCheckConstraint(constraintName, sql));
		}
	}

	private static string GetColumnName<T>(EntityTypeBuilder<T> builder, MemberInfo? member) where T : class
	{
		ArgumentNullException.ThrowIfNull(member);

		var table = StoreObjectIdentifier.Create(
			builder.Metadata,
			StoreObjectType.Table);

		if (table is not {} t)
		{
			throw new InvalidOperationException($"Entity type '{builder.Metadata.DisplayName()}' is not mapped to a table");
		}

		var prop = builder.Metadata.FindProperty(member);
		if (prop is null)
		{
			throw new InvalidOperationException($"Property '{member.Name}' is not mapped to table '{t.Name}'.");
		}

		var name = prop.GetColumnName(t);
		if (name is null)
		{
			throw new InvalidOperationException($"Property '{prop.Name}' is not mapped to table '{t.Name}'");
		}

		return name;
	}
}