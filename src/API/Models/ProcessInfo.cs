namespace VessieFramework.Models;

public class ProcessInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint PriorityClass { get; set; }
    public IntPtr AffinityMask { get; set; }
    public IntPtr WorkingSetMin { get; set; }
    public IntPtr WorkingSetMax { get; set; }
    public double CpuUsage { get; set; }
    public long MemoryUsage { get; set; }
}