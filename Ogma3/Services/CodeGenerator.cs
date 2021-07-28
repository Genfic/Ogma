using System;

namespace Ogma3.Services
{
    public static class CodeGenerator
    {
        public static string InviteCode()
        {
            var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var unixStr = unix.ToString("0000000000")[10..];

            var random = new Random();
            var bytes = new byte[5];
            random.NextBytes(bytes);

            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = string.Concat(hexArray);
            
            return unixStr + hexStr;
        }
    }
}