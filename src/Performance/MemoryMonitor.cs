using System.Diagnostics;

namespace VessieFramework.Monitors;

public static class MemoryMonitor
{
    private static readonly PerformanceCounter _memAvailable = new("Memory", "Available MBytes");
    
    public static float GetAvailableMemoryMB() => _memAvailable.NextValue();

    public static long GetProcessMemoryMB(int pid)
    {
        try { return Process.GetProcessById(pid).WorkingSet64 / 1024 / 1024; }
        catch { return 0; }
    }
}