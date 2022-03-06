namespace AvatarService.Generators;

public static class TextGenerators
{
    public static string Initials(string name) => string.Join(string.Empty, name
        .Split(' ', '_', '-')
        .Select(s => s.Trim().ToUpper()[0]));
}