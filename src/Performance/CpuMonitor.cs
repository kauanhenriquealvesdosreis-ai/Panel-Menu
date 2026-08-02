using System.Diagnostics;

namespace VessieFramework.Monitors;

public static class CpuMonitor
{
    private static readonly PerformanceCounter _totalCpu = new("Processor", "% Processor Time", "_Total");

    public static float GetTotalUsage() => _totalCpu.NextValue();

    public static float GetProcessUsage(int pid)
    {
        try
        {
            var counter = new PerformanceCounter("Process", "% Processor Time", 
                Process.GetProcessById(pid).ProcessName, true);
            return counter.NextValue();
        }
        catch { return 0; }
    }
}