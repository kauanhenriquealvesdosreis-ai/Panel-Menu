using VessieFramework.Native;

namespace VessieFramework.Optimizer;

public class Profile
{
    public string Name { get; set; } = "Balanced";
    public uint Priority { get; set; } = NativeMethods.NORMAL_PRIORITY_CLASS;
    public IntPtr AffinityMask { get; set; } = new IntPtr(0xFFFFFFFF);
    public IntPtr WorkingSetMin { get; set; } = new IntPtr(0);
    public IntPtr WorkingSetMax { get; set; } = new IntPtr(0);

    public static Profile Gaming()
    {
        return new Profile
        {
            Name = "Gaming",
            Priority = NativeMethods.ABOVE_NORMAL_PRIORITY_CLASS,
            AffinityMask = new IntPtr(0xFFFFFFFF)
        };
    }

    public static Profile Workstation()
    {
        return new Profile { Name = "Workstation", Priority = NativeMethods.NORMAL_PRIORITY_CLASS };
    }

    public static Profile Background()
    {
        return new Profile { Name = "Background", Priority = NativeMethods.BELOW_NORMAL_PRIORITY_CLASS };
    }
}