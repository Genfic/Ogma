using System;
using System.Security.Cryptography;

namespace Ogma3.Services.CodeGenerator;

public class CodeGenerator : ICodeGenerator
{
    public string GetInviteCode()
    {
        var unix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var unixStr = unix.ToString("0000000000")[10..];

        var bytes = RandomNumberGenerator.GetBytes(5);

        var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
        var hexStr = string.Concat(hexArray);
            
        return unixStr + hexStr;
    }
}