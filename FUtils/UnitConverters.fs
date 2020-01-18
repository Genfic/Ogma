namespace Utils

module UnitConverters =

    let FormatBytes bytes : string =
        if   bytes >= 0x1000000000000000L then ((double)(bytes >>> 50) / 1024.0).ToString("0.### EB"); 
        elif bytes >= 0x4000000000000L then ((double)(bytes >>> 40) / 1024.0).ToString("0.### PB"); 
        elif bytes >= 0x10000000000L then ((double)(bytes >>> 30) / 1024.0).ToString("0.### TB"); 
        elif bytes >= 0x40000000L then ((double)(bytes >>> 20) / 1024.0).ToString("0.### GB"); 
        elif bytes >= 0x100000L then ((double)(bytes >>> 10) / 1024.0).ToString("0.### MB"); 
        elif bytes >= 0x400L then ((double)bytes / 1024.0).ToString("0.### KB"); 
        else bytes.ToString("0 Bytes");