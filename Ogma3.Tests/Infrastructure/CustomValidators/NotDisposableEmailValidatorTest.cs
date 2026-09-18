using FluentValidation;
using Ogma3.Infrastructure.CustomValidators;
using Ogma3.Services.EmailBlocklistProvider;

namespace Ogma3.Tests.Infrastructure.CustomValidators;

public sealed class NotDisposableEmailValidatorTest
{
	private static readonly StubProvider Provider = new("mailinator.com", "tempmail.com");

	private readonly NotDisposableEmailValidator<int> _validator = new(Provider);

	[Test]
	public async Task TestIsValid_Null()
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), null)).IsTrue();
	}

	[Test]
	[Arguments("")]
	[Arguments("   ")]
	[Arguments("user@gmail.com")]
	[Arguments("UsEr@GmAiL.CoM")]
	[Arguments("no-at-sign")]
	[Arguments("user@")]
	[Arguments("user@sub.mailinator.com")]
	public async Task TestIsValid_Valid(string? value)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), value)).IsTrue();
	}

	[Test]
	[Arguments("user@mailinator.com")]
	[Arguments("user@Mailinator.com")]
	[Arguments("user@tempmail.com")]
	public async Task TestIsValid_Invalid(string value)
	{
		await Assert.That(_validator.IsValid(new ValidationContext<int>(0), value)).IsFalse();
	}

	[Test]
	public async Task TestName()
	{
		await Assert.That(_validator.Name).IsEqualTo("NotDisposableEmailValidator");
	}

	private sealed class Dto
	{
		public string? Email { get; set; }
	}

	[Test]
	public async Task TestErrorMessage_DisposableEmail()
	{
		var validator = new InlineValidator<Dto>();
		validator.RuleFor(d => d.Email).NotDisposable(Provider);

		var result = validator.Validate(new Dto { Email = "user@mailinator.com" });

		await Assert.That(result.IsValid).IsFalse();
		await Assert.That(result.Errors).IsNotEmpty();
		await Assert.That(result.Errors[0].ErrorMessage).IsEqualTo("Email not supported.");
	}

	private sealed class StubProvider(params string[] disposableDomains) : IEmailBlocklistProvider
	{
		public bool IsDisposable(string email)
			=> disposableDomains.Contains(email, StringComparer.OrdinalIgnoreCase);
	}
}