using System.Diagnostics;

namespace VessieFramework.Performance;

public static class CpuMonitor
{
    private static readonly PerformanceCounter? _totalCpu;
    private static bool _isWindows = OperatingSystem.IsWindows();

    static CpuMonitor()
    {
        if (_isWindows)
        {
            try
            {
                _totalCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            catch
            {
                _totalCpu = null;
            }
        }
    }

    public static float GetTotalUsage()
    {
        if (!_isWindows || _totalCpu == null)
        {
            return GetLinuxCpuUsage();
        }
        return _totalCpu.NextValue();
    }

    public static float GetProcessUsage(int pid)
    {
        try
        {
            if (_isWindows && _totalCpu != null)
            {
                var counter = new PerformanceCounter("Process", "% Processor Time",
                    Process.GetProcessById(pid).ProcessName, true);
                return counter.NextValue();
            }
            else
            {
                var process = Process.GetProcessById(pid);
                return (float)process.TotalProcessorTime.TotalMilliseconds / 
                       (float)Environment.TickCount * 100;
            }
        }
        catch { return 0; }
    }

    private static float GetLinuxCpuUsage()
    {
        try
        {
            var statPath = "/proc/stat";
            if (!File.Exists(statPath)) return 0;

            var lines = File.ReadAllLines(statPath);
            foreach (var line in lines)
            {
                if (line.StartsWith("cpu "))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 5)
                    {
                        var user = long.Parse(parts[1]);
                        var nice = long.Parse(parts[2]);
                        var system = long.Parse(parts[3]);
                        var idle = long.Parse(parts[4]);
                        var total = user + nice + system + idle;
                        var used = user + nice + system;
                        return (float)used / total * 100;
                    }
                }
            }
        }
        catch { }
        return 0;
    }
}
