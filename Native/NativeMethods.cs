using System.Runtime.InteropServices;

namespace VessieFramework.Native;

public static class NativeMethods
{
    public const uint IDLE_PRIORITY_CLASS = 0x40;
    public const uint BELOW_NORMAL_PRIORITY_CLASS = 0x4000;
    public const uint NORMAL_PRIORITY_CLASS = 0x20;
    public const uint ABOVE_NORMAL_PRIORITY_CLASS = 0x8000;
    public const uint HIGH_PRIORITY_CLASS = 0x80;
    public const uint REALTIME_PRIORITY_CLASS = 0x100;

    public const uint PROCESS_QUERY_INFORMATION = 0x0400;
    public const uint PROCESS_SET_INFORMATION = 0x0200;
    public const uint PROCESS_SET_LIMITED_INFORMATION = 0x2000;
    public const uint PROCESS_TERMINATE = 0x0001;

    public const uint QUOTA_LIMITS_HARDWS_MIN_ENABLE = 0x00000001;
    public const uint QUOTA_LIMITS_HARDWS_MAX_DISABLE = 0x00000002;
    public const uint QUOTA_LIMITS_USE_DEFAULT_LIMITS = 0x00000004;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetPriorityClass(IntPtr hProcess, uint dwPriorityClass);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetPriorityClass(IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetProcessAffinityMask(IntPtr hProcess, IntPtr dwProcessAffinityMask);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetProcessAffinityMask(IntPtr hProcess, out IntPtr lpProcessAffinityMask, out IntPtr lpSystemAffinityMask);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetProcessWorkingSetSizeEx(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize, uint Flags);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetProcessWorkingSetSizeEx(IntPtr hProcess, out IntPtr lpMinimumWorkingSetSize, out IntPtr lpMaximumWorkingSetSize, out uint Flags);
}