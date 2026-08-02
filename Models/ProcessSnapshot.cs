namespace VessieFramework.Models;

public class ProcessSnapshot
{
    public int ProcessId { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public uint OriginalPriority { get; set; }
    public IntPtr OriginalAffinity { get; set; }
    public IntPtr OriginalWorkingSetMin { get; set; }
    public IntPtr OriginalWorkingSetMax { get; set; }
    public DateTime SnapshotTime { get; set; }
    public bool IsRestored { get; set; }
}