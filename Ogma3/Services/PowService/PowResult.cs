namespace Ogma3.Services.PowService;

public sealed record PowResult(string Token, int Nonce, string Hash);