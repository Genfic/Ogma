namespace Ogma3.Services.CodeGenerator;

public interface ICodeGenerator
{
	string GetInviteCode(bool isBase64 = false);
}