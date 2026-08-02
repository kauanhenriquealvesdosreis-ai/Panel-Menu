using System.Diagnostics;
using VessieFramework.Native;
using VessieFramework.Models;

namespace VessieFramework.Core;

public static class ProcessManager
{
    public static List<ProcessInfo> GetRunningProcesses()
    {
        var list = new List<ProcessInfo>();
        foreach (var p in Process.GetProcesses())
        {
            try
            {
                if (p.Id == 0 || string.IsNullOrEmpty(p.ProcessName)) continue;
                var handle = NativeMethods.OpenProcess(
                    NativeMethods.PROCESS_QUERY_INFORMATION | NativeMethods.PROCESS_SET_INFORMATION,
                    false, p.Id);

                if (handle == IntPtr.Zero) continue;

                var info = new ProcessInfo
                {
                    Id = p.Id,
                    Name = p.ProcessName,
                    PriorityClass = NativeMethods.GetPriorityClass(handle),
                    MemoryUsage = p.WorkingSet64 / 1024 / 1024
                };

                if (NativeMethods.GetProcessAffinityMask(handle, out IntPtr procMask, out _))
                    info.AffinityMask = procMask;

                if (NativeMethods.GetProcessWorkingSetSizeEx(handle, out IntPtr min, out IntPtr max, out _))
                {
                    info.WorkingSetMin = min;
                    info.WorkingSetMax = max;
                }

                NativeMethods.CloseHandle(handle);
                list.Add(info);
            }
            catch { }
        }
        return list;
    }

    public static bool SetPriority(int pid, uint priorityClass)
    {
        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;
        var result = NativeMethods.SetPriorityClass(handle, priorityClass);
        NativeMethods.CloseHandle(handle);
        return result;
    }

    public static bool SetAffinity(int pid, IntPtr mask)
    {
        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;
        var result = NativeMethods.SetProcessAffinityMask(handle, mask);
        NativeMethods.CloseHandle(handle);
        return result;
    }

    public static bool SetWorkingSet(int pid, IntPtr min, IntPtr max)
    {
        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_SET_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return false;
        var result = NativeMethods.SetProcessWorkingSetSizeEx(handle, min, max,
            NativeMethods.QUOTA_LIMITS_HARDWS_MIN_ENABLE | NativeMethods.QUOTA_LIMITS_HARDWS_MAX_DISABLE);
        NativeMethods.CloseHandle(handle);
        return result;
    }

    public static uint GetPriority(int pid)
    {
        var handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_INFORMATION, false, pid);
        if (handle == IntPtr.Zero) return 0;
        var prio = NativeMethods.GetPriorityClass(handle);
        NativeMethods.CloseHandle(handle);
        return prio;
    }
}