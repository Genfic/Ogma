namespace AvatarService.Infrastructure;

public static class StringExtensions
{
    // Code shamelessly taken from https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
    public static int GetDeterministicHashCode(this string str)
    {
        var span = str.AsSpan();

        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;

            for (var i = 0; i < span.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ span[i];
                if (i == span.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ span[i + 1];
            }

            return hash1 + hash2 * 1566083941;
        }
    }
}