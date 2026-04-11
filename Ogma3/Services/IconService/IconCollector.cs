using JetBrains.Annotations;

namespace Ogma3.Services.IconService;

[RegisterScoped]
[UsedImplicitly]
public sealed class IconCollector
{
	public HashSet<string> RequestedIcons { get; } = [];

	public void RegisterIcon(string name) => RequestedIcons.Add(name);
}