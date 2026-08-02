using System.Collections.Concurrent;
using System.Diagnostics;
using VessieFramework.Models;
using VessieFramework.Native;

namespace VessieFramework.Core;

public static class RollbackManager
{
    private static readonly ConcurrentDictionary<int, ProcessSnapshot> _snapshots = new();

    public static void TakeSnapshot(int pid)
    {
        var handle = NativeMethods.OpenProcess(
            NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return;

        try
        {
            string processName = "Unknown";
            try { processName = Process.GetProcessById(pid).ProcessName; } catch { }

            var snap = new ProcessSnapshot
            {
                ProcessId = pid,
                ProcessName = processName,
                OriginalPriority = NativeMethods.GetPriorityClass(handle),
                SnapshotTime = DateTime.Now,
                IsRestored = false
            };

            NativeMethods.GetProcessAffinityMask(handle, out IntPtr mask, out _);
            snap.OriginalAffinity = mask;

            NativeMethods.GetProcessWorkingSetSizeEx(handle, out IntPtr min, out IntPtr max, out _);
            snap.OriginalWorkingSetMin = min;
            snap.OriginalWorkingSetMax = max;

            _snapshots[pid] = snap;
        }
        finally { NativeMethods.CloseHandle(handle); }
    }

    public static bool RestoreSnapshot(int pid)
    {
        if (!_snapshots.TryGetValue(pid, out var snap) || snap.IsRestored)
            return false;

        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;

        try
        {
            NativeMethods.SetPriorityClass(handle, snap.OriginalPriority);
            NativeMethods.SetProcessAffinityMask(handle, snap.OriginalAffinity);
            NativeMethods.SetProcessWorkingSetSizeEx(handle,
                snap.OriginalWorkingSetMin, snap.OriginalWorkingSetMax,
                NativeMethods.QUOTA_LIMITS_HARDWS_MIN_ENABLE | NativeMethods.QUOTA_LIMITS_HARDWS_MAX_DISABLE);

            snap.IsRestored = true;
            return true;
        }
        finally { NativeMethods.CloseHandle(handle); }
    }

    public static void RestoreAllAndClear()
    {
        foreach (var pid in _snapshots.Keys.ToList())
        {
            RestoreSnapshot(pid);
        }
        _snapshots.Clear();
    }

    public static bool IsTracked(int pid) => _snapshots.ContainsKey(pid);
    public static List<ProcessSnapshot> GetAllSnapshots() => _snapshots.Values.ToList();
    public static void RemoveSnapshot(int pid) => _snapshots.TryRemove(pid, out _);
}