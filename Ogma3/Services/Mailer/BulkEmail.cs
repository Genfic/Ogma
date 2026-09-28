using PostmarkDotNet;

namespace Ogma3.Services.Mailer;

public sealed record BulkEmail(
	string Email,
	string TemplateName,
	Dictionary<string, string> Model,
	List<PostmarkMessageAttachment>? Attachments = null
);