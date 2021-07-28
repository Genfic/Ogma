using System;

namespace Ogma3.Services.CodeGenerator
{
    public class CodeGenerator : ICodeGenerator
    {
        private readonly Random _rng;
        public CodeGenerator() => _rng = new Random();

        public string GetInviteCode()
        {
            var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var unixStr = unix.ToString("0000000000")[10..];

            var bytes = new byte[5];
            _rng.NextBytes(bytes);

            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = string.Concat(hexArray);
            
            return unixStr + hexStr;
        }
    }
}