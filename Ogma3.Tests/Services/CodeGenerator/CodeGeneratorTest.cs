using CodeGen = Ogma3.Services.CodeGenerator.CodeGenerator;

namespace Ogma3.Tests.Services.CodeGenerator;

public sealed class CodeGeneratorTest
{
	private const string CrockfordAlphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

	[Test]
	public async Task TestGetInviteCodeLength()
	{
		var code = new CodeGen().GetInviteCode();
		await Assert.That(code.Length).IsEqualTo(16);
	}

	[Test]
	public async Task TestGetInviteCodeIsUpperCaseCrockfordBase32()
	{
		var code = new CodeGen().GetInviteCode();
		await Assert.That(code).IsEqualTo(code.ToUpperInvariant());
		foreach (var c in code)
		{
			await Assert.That(CrockfordAlphabet.Contains(c)).IsTrue();
		}
	}

	[Test]
	public async Task TestGetInviteCodesAreUnique()
	{
		var generator = new CodeGen();
		var codes = Enumerable.Range(0, 100).Select(_ => generator.GetInviteCode()).ToHashSet();
		await Assert.That(codes.Count).IsEqualTo(100);
	}
}