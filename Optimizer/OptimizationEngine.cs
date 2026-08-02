using VessieFramework.Core;
using VessieFramework.Models;

namespace VessieFramework.Optimizer;

public static class OptimizationEngine
{
    public static bool ApplyProfile(int pid, Profile profile)
    {
        if (!RollbackManager.IsTracked(pid))
            RollbackManager.TakeSnapshot(pid);

        var success = true;
        if (!ProcessManager.SetPriority(pid, profile.Priority)) success = false;
        if (!ProcessManager.SetAffinity(pid, profile.AffinityMask)) success = false;
        if (!ProcessManager.SetWorkingSet(pid, profile.WorkingSetMin, profile.WorkingSetMax)) success = false;

        return success;
    }

    public static bool RestoreProcess(int pid)
    {
        return RollbackManager.RestoreSnapshot(pid);
    }
}