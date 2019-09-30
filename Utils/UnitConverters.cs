using System;

namespace Utils
{
    public class UnitConverters
    {
        public static string FormatBytes(long bytes)
        {
            if (bytes >= 0x1000000000000000)  return ((double)(bytes >> 50) / 1024).ToString("0.### EB"); 
            if (bytes >= 0x4000000000000)  return ((double)(bytes >> 40) / 1024).ToString("0.### PB"); 
            if (bytes >= 0x10000000000)  return ((double)(bytes >> 30) / 1024).ToString("0.### TB"); 
            if (bytes >= 0x40000000)  return ((double)(bytes >> 20) / 1024).ToString("0.### GB"); 
            if (bytes >= 0x100000)  return ((double)(bytes >> 10) / 1024).ToString("0.### MB"); 
            if (bytes >= 0x400)  return ((double)bytes / 1024).ToString("0.###") + " KB"; 
            return bytes.ToString("0 Bytes");
        }
    }
}