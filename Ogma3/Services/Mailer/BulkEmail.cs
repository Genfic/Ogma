namespace Ogma3.Services.Mailer;

public sealed record BulkEmail(string Email, string TemplateName, Dictionary<string, string> Model);