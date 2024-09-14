using FluentValidation;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Ogma3.Data;
using Ogma3.Infrastructure.ServiceRegistrations;

namespace Ogma3.Api.V1.Roles;

using ReturnType = Results<Ok, NotFound>;

[Handler]
[MapPut("api/roles")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public static partial class UpdateRole
{
	public sealed record Command(long Id, string Name, bool IsStaff, string Color, byte Order);

	public sealed class CommandValidator : AbstractValidator<Command>
	{
		public CommandValidator() => RuleFor(r => r.Name).NotEmpty();
	}
	
	private static async ValueTask<ReturnType> HandleAsync(
		Command request,
		ApplicationDbContext context,
		CancellationToken cancellationToken
	)
	{
		var rows = await context.Roles
			.Where(r => r.Id == request.Id)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(r =>r.Name, request.Name)
				.SetProperty(r =>r.Order, request.Order)
				.SetProperty(r => r.IsStaff, request.IsStaff)
				.SetProperty(r =>r.Color, request.Color),
			cancellationToken);

		return rows > 0 ? TypedResults.Ok() : TypedResults.NotFound();
	}
}