using System.Diagnostics;

namespace VessieFramework.Performance;

public static class MemoryMonitor
{
    public static long GetAvailableMemoryMB()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                var counter = new PerformanceCounter("Memory", "Available MBytes");
                return (long)counter.NextValue();
            }
            catch
            {
                return GetLinuxAvailableMemory();
            }
        }
        else
        {
            return GetLinuxAvailableMemory();
        }
    }

    public static long GetTotalMemoryMB()
    {
        return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
    }

    private static long GetLinuxAvailableMemory()
    {
        try
        {
            var meminfoPath = "/proc/meminfo";
            if (!File.Exists(meminfoPath)) return 0;

            var lines = File.ReadAllLines(meminfoPath);
            foreach (var line in lines)
            {
                if (line.StartsWith("MemAvailable:"))
                {
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out var kb))
                    {
                        return kb / 1024; // Convert KB to MB
                    }
                }
            }
        }
        catch { }
        return 0;
    }

    public static float GetMemoryUsagePercent()
    {
        var total = GetTotalMemoryMB();
        var available = GetAvailableMemoryMB();
        if (total == 0) return 0;
        return (float)(total - available) / total * 100;
    }
}
