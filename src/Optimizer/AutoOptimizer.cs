using System.Diagnostics;
using VessieFramework.Monitors;
using VessieFramework.Core;
using VessieFramework.Native;

namespace VessieFramework.Optimizer;

public static class AutoOptimizer
{
    private static readonly List<string> _gameProcesses = new() { "csgo", "valorant", "fortnite", "leagueoflegends", "overwatch", "cyberpunk2077", "gta5" };
    private static readonly List<string> _editingProcesses = new() { "premiere", "aftereffects", "davinci", "resolve", "vegas" };

    public static Profile DetectAndGetProfile(string processName)
    {
        var lower = processName.ToLower();

        if (_gameProcesses.Any(p => lower.Contains(p)))
            return Profile.Gaming();

        if (_editingProcesses.Any(p => lower.Contains(p)))
            return new Profile { Name = "Editing", Priority = NativeMethods.ABOVE_NORMAL_PRIORITY_CLASS };

        if (CpuMonitor.GetTotalUsage() > 80 && MemoryMonitor.GetAvailableMemoryMB() < 4096)
            return Profile.Background();

        return Profile.Workstation();
    }

    public static void RunAutoOptimization()
    {
        var processes = ProcessManager.GetRunningProcesses();
        foreach (var p in processes.Take(10))
        {
            if (p.Id == 0 || p.Id == Process.GetCurrentProcess().Id) continue;

            var profile = DetectAndGetProfile(p.Name);
            var currentPrio = ProcessManager.GetPriority(p.Id);

            if (currentPrio != profile.Priority || !RollbackManager.IsTracked(p.Id))
            {
                OptimizationEngine.ApplyProfile(p.Id, profile);
                Utils.Logger.Log($"Auto-Optimized {p.Name} (PID {p.Id}) -> {profile.Name}");
            }
        }
    }
}