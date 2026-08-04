using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ogma3.Data.Helpers;

public static class EntityBuilderExtensions
{
	public static IndexBuilder HasPartialIndex<TEntity>(
		this EntityTypeBuilder<TEntity> indexBuilder,
		Expression<Func<TEntity, object?>> indexExpression
	) where TEntity : class
	{
		IReadOnlyList<MemberExpression> members = indexExpression.Body switch
		{
			MemberExpression m => [m],
			UnaryExpression { Operand: MemberExpression m, NodeType: ExpressionType.Convert } => [m],
			NewExpression n => n.Arguments
				.Select(a => a switch
				{
					MemberExpression m => m,
					UnaryExpression {Operand: MemberExpression m, NodeType: ExpressionType.Convert } => m,
					_ => throw new NotSupportedException($"Expression {a} is not supported")
				})
				.ToList(),
			_ => throw new NotSupportedException($"Expression {indexExpression} is not supported"),
		};

		var sql = string.Join("AND", members.Select(member => $"\"{member.Member.Name}\" IS NOT NULL"));

		return indexBuilder.HasIndex(indexExpression).HasFilter(sql);
	}
}